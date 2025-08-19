using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PEI.Assesment;


/// <summary>
/// This class defines an Azure Function that processes order details.
/// It retrieves order information from the request body, constructs a payload, 
/// sends it to an D365 API, and creates a record in a custom table in Dynamics 365.
/// The function expects a JSON payload with CustomerName, OrderTotal, and OrderDate.   
/// /// It retrieves an access token for Dynamics 365 and creates a record in a custom table named "Order details azure".
/// If successful, it returns a success message; otherwise, it logs the error and returns a failure message.
/// The function is triggered by an HTTP request.
/// </summary>
public class PEIMoveOderDetails
{
    private readonly ILogger<PEIMoveOderDetails> _logger;

    public PEIMoveOderDetails(ILogger<PEIMoveOderDetails> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Function runs when an HTTP request is received.
    /// It processes the order details and sends them to an external API.
    /// Here it is creating a new record in a custom table in Dynamics 365.
    /// The function expects a JSON payload with CustomerName, OrderTotal, and OrderDate.
    ///It retrieves an access token for Dynamics 365 and Creates a record in custom table name Order details azure.
    ///If successful, it returns a success message; otherwise, it logs the error and returns a failure message.
    /// </summary>
    /// <param name="req"></param>
    /// <returns>string message</returns>
    [Function("PEIMoveOderDetails")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic order = JsonConvert.DeserializeObject(requestBody);
        try
        {
            string customerName = order?.CustomerName;
            decimal orderTotal = order?.OrderTotal;
            string orderDate = order?.OrderDate;

            var apiPayload = new
            {
                pei_customername = customerName,
                pei_ordertotal = orderTotal,
                pei_orderdate = orderDate
            };

            string access_token = await GetDynamicsTokenAsync(null, null, null, Environment.GetEnvironmentVariable("CrmUrl"));
            string jsonPayload = JsonConvert.SerializeObject(apiPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            //string mockApiUrl = "https://mockapi.example.com/orders";
            string mockApiUrl = Environment.GetEnvironmentVariable("CrmApiUrl");
            //HttpClient httpClient = new HttpClient();

            //HttpResponseMessage response = await httpClient.PostAsync(mockApiUrl, content);

            string response = await CreateOrderDetailsAsync(mockApiUrl, access_token, jsonPayload);


            if (!string.IsNullOrEmpty(response) && !response.Contains("Failed"))
            {
                _logger.LogInformation("Successfully sent order to external API.");
                return new OkObjectResult("Order processed successfully." + response);
            }
            else
            {
                _logger.LogError($"Failed to send order. Error:{response}");
                return new OkObjectResult(response);
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<string> GetDynamicsTokenAsync(
        string tenantId, string clientId, string clientSecret, string resourceUrl)
    {
        tenantId = Environment.GetEnvironmentVariable("TenantId");
        clientId = Environment.GetEnvironmentVariable("ClientId");
        clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        string authority = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

        using (var httpClient = new HttpClient())
        {
            var body = new StringContent(
                $"client_id={clientId}&scope={resourceUrl}/.default&client_secret={clientSecret}&grant_type=client_credentials",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );

            HttpResponseMessage response = await httpClient.PostAsync(authority, body);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get token. Status: {response.StatusCode}, Error: {await response.Content.ReadAsStringAsync()}");
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result.access_token;
        }
    }

    public static async Task<string> CreateOrderDetailsAsync(string d365Url, string accessToken, object contact)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string jsonPayload = JsonConvert.SerializeObject(contact);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            string apiUrl = $"{d365Url}/api/data/v9.2/pei_orderdetailsazures"; // Creating a record in Custom table

            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                return "Order Details created successfully. Location: " + response.Headers.Location;
            }
            else
            {
                return $"Failed to create Order Details. Status: {response.StatusCode}, Error: {await response.Content.ReadAsStringAsync()}";
            }
        }
    }
}