using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Deleteanaplan
{
    public class DeleteRelatedRecords : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Check if the input parameters contain the target entity.
            if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is EntityReference targetEntity))
            {
                return;
            }

            // Get the organization service.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Example: Deleting related records for a custom entity "anaplnaoutput".
                if (targetEntity.LogicalName == "zx_anaplanoutput")
                {
                    // Query for related records. Update the "related_entity_name" and relationship field as needed.
                    var query = new QueryExpression("zx_budgetdetail")
                    {
                        ColumnSet = new ColumnSet(false),
                        Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("zx_anaplan", ConditionOperator.Equal, targetEntity.Id)
                            }
                        }
                    };

                    var relatedRecords = service.RetrieveMultiple(query);

                    // Delete related records.
                    foreach (var relatedRecord in relatedRecords.Entities)
                    {
                        service.Delete(relatedRecord.LogicalName, relatedRecord.Id);
                    }


                    var query1 = new QueryExpression("zx_budget")
                    {
                        ColumnSet = new ColumnSet(false),
                        Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("zx_anaplan", ConditionOperator.Equal, targetEntity.Id)
                            }
                        }
                    };

                    var relatedRecords1 = service.RetrieveMultiple(query1);

                    // Delete related records.
                    foreach (var relatedRecord1 in relatedRecords1.Entities)
                    {
                        service.Delete(relatedRecord1.LogicalName, relatedRecord1.Id);
                    }




                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in the DeleteRelatedRecords plugin: {ex.Message}", ex);
            }
        }
    }
}
