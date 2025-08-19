using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Timer;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

/// <summary>
/// This class defines an Azure Function that polls for changes in sales orders.
/// It retrieves changes using change tracking, processes new or updated orders,
/// and sends the details to an external API. In our case its an HTTP trigered function Azure Function PEIMoveOderDetails.
/// The function is triggered by a timer and runs every 5 minutes.
/// It uses change tracking to efficiently poll for changes in the sales order entity.
/// The function expects the environment variables for CRM connection and API URL to be set.
/// It logs the changes and sends the order details to an HTTP Azure function.
/// If successful, it logs the success; otherwise, it logs the error.
/// </summary>
public class PollOrdersChangeTracking
{
    private readonly ILogger _logger;
    private static string lastToken = null; // Store in persistent storage in production

    public PollOrdersChangeTracking(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PollOrdersChangeTracking>();
    }

    [Function("PollOrdersChangeTracking")]
    public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        try
        {
            var connStr = $"AuthType=ClientSecret;Url={Environment.GetEnvironmentVariable("CrmUrl")};" +
                          $"ClientId={Environment.GetEnvironmentVariable("ClientId")};" +
                          $"ClientSecret={Environment.GetEnvironmentVariable("ClientSecret")};" +
                          $"TenantId={Environment.GetEnvironmentVariable("TenantId")};";

            string mockApiUrl = Environment.GetEnvironmentVariable("PEIMoveOrderDetails");

            using var serviceClient = new ServiceClient(connStr);

            var query = new QueryExpression("salesorder")
            {
                ColumnSet = new ColumnSet("customerid", "totalamount", "submitdate"),
                PageInfo = new PagingInfo { Count = 5000, PageNumber = 1 }
            };

            var changesRequest = new RetrieveEntityChangesRequest
            {
                EntityName = "salesorder",
                Columns = query.ColumnSet,
                PageInfo = query.PageInfo
            };

            if (!string.IsNullOrEmpty(lastToken))
            {
                changesRequest.DataVersion = lastToken; // Resume from last checkpoint
            }

            var changesResponse = (RetrieveEntityChangesResponse)serviceClient.Execute(changesRequest);

            foreach (var change in changesResponse.EntityChanges.Changes)
            {
                if (change is NewOrUpdatedItem newOrUpdated)
                {
                    Entity order = newOrUpdated.NewOrUpdatedEntity.ToEntity<Entity>();
                    EntityReference customer = order.Contains("customerid") ? order.GetAttributeValue<EntityReference>("customerid") : null;

                    Entity potentialCustomer = customer != null ? serviceClient.Retrieve(customer.LogicalName, customer.Id, new ColumnSet("name")) : null;
                    string customerName = potentialCustomer != null && potentialCustomer.Contains("name") ? potentialCustomer.GetAttributeValue<string>("name") : string.Empty;

                    var payload = new
                    {
                        CustomerName = customerName,
                        OrderTotal = order.GetAttributeValue<Money>("totalamount")?.Value,
                        OrderDate = order.GetAttributeValue<DateTime>("submitdate")
                    };

                    using var client = new HttpClient();
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(mockApiUrl, jsonContent);

                    if (response.IsSuccessStatusCode)
                        _logger.LogInformation($" Sent order {payload.CustomerName}");
                    else
                        _logger.LogError($" Failed to send order {payload.CustomerName}: {response.StatusCode}");
                }
                else if (change is RemovedOrDeletedItem deletedItem)
                {
                    _logger.LogInformation($" Order deleted: {deletedItem.RemovedItem.Id}");
                }
            }

            // Save the new data version (token)
            lastToken = changesResponse.EntityChanges.DataToken;

            _logger.LogInformation("Change tracking token updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
    }

    // Timer info model for isolated process
    public class TimerInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }
        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }
        public DateTime Next { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}