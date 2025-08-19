using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PEI.Assesment.Plugins
{
    public class PostCreateAccount : IPlugin
    {
        public void Execute (IServiceProvider serviceProvider)
        {
            // Plugin execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Check if the target is an Account entity
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity account && account.LogicalName == "account")
            {
                try
                {
                    // Get Account Name
                    string accountName = account.Contains("name") ? account["name"].ToString() : "Unknown";

                    // Create Contact entity
                    Entity contact = new Entity("contact");
                    contact["firstname"] = "Default";
                    contact["lastname"] = accountName;
                    contact["parentcustomerid"] = new EntityReference("account", account.Id);

                    // Create the Contact record
                    service.Create(contact);

                    // Log success message
                    tracingService.Trace($"Contact created successfully for Account '{accountName}' with First Name 'Default'.");
                }
                catch (Exception ex)
                {
                    tracingService.Trace("Error in PostCreateAccount: " + ex.Message);
                    throw new InvalidPluginExecutionException("An error occurred while creating the default contact.", ex);
                }
            }

        }
    }
}
