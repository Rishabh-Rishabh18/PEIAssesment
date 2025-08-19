using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PEI.Assesment.Plugins
{
    public class PreValidateCContact : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // Check if the input parameters contain a target entity
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity && entity.LogicalName == "contact")
            {
                if (entity.Attributes.Contains("emailaddress1"))
                {
                    string email = entity["emailaddress1"].ToString().TrimStart().TrimEnd();

                    // Use QueryExpression to search for existing contacts with the same email
                    QueryExpression query = new QueryExpression("contact")
                    {
                        ColumnSet = new ColumnSet("contactid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                        {
                            new ConditionExpression("emailaddress1", ConditionOperator.Equal, email)
                        }
                        },
                        TopCount = 1 // Efficient for large datasets
                    };

                    EntityCollection results = service.RetrieveMultiple(query);

                    if (results.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("A contact with this email address already exists.");
                    }
                }
            }
        }
    }
}

