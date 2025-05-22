using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anaplan_Output
{
    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);

            if (context.Depth == 1)
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity AnaplanOutput = (Entity)context.InputParameters["Target"];
                    AnaplanOutput = service.Retrieve("zx_anaplanoutput", AnaplanOutput.Id, new ColumnSet("zx_variant", "zx_brand", "zx_category"));
                   if (AnaplanOutput.Contains("zx_variant"))
                    {
                    Guid zx_variant = ((EntityReference)AnaplanOutput.Attributes["zx_variant"]).Id;
                    Entity variant = service.Retrieve("zx_variant", zx_variant, new ColumnSet("zx_category", "zx_brand"));
                     if(variant.Contains("zx_brand"))
                        {
                            Guid zx_brand1 = ((EntityReference)variant.Attributes["zx_brand"]).Id;
                            Entity zx_brand = service.Retrieve("zx_brand", zx_brand1, new ColumnSet("zx_category"));

                            if(variant.Contains("zx_category"))
                            {
                                AnaplanOutput.Attributes["zx_category"] = zx_brand.Attributes["zx_category"];
                                AnaplanOutput.Attributes["zx_brand"] = variant.Attributes["zx_brand"];
                                service.Update(AnaplanOutput);
                            }
                            else
                            {
                                variant.Attributes["zx_category"] = zx_brand.Attributes["zx_category"];
                             
                                service.Update(variant);

                                AnaplanOutput.Attributes["zx_category"] = zx_brand.Attributes["zx_category"];
                                AnaplanOutput.Attributes["zx_brand"] = variant.Attributes["zx_brand"];
                                service.Update(AnaplanOutput);

                            }




                        }


                    }
                    else
                    {
                        AnaplanOutput.Attributes["zx_category"] = null;
                        AnaplanOutput.Attributes["zx_brand"] = null;
                        service.Update(AnaplanOutput);
                    }
             /*  Entity  AnaplanOutput1 = service.Retrieve("zx_anaplanoutput", AnaplanOutput.Id, new ColumnSet("zx_variant", "zx_brand", "zx_category", "zx_request"));
                 
                    if(AnaplanOutput1.Contains("zx_variant"))
                    {
                     
                    string approvalrequest = $@"<fetch version='1.0' mapping='logical' savedqueryid='60dc0487-341f-444b-bce9-34dd4dfd98ee' no-lock='false' distinct='true'>
                                            <entity name='zx_budgetapprovalrequest'>
                                            <attribute name='zx_budgetapprovalrequestid'/>
                                            <attribute name='zx_name'/>
                                            <attribute name='createdon'/>
                                            <order attribute='zx_name' descending='false'/>
                                            <attribute name='zx_category'/>
                                            <attribute name='zx_centeralspoc'/>
                                            <attribute name='zx_centralspocstatus'/>
                                            <attribute name='zx_mastatus'/>
                                            <attribute name='zx_mhstatus'/>
                                            <attribute name='zx_marketinghead'/>
                                            <attribute name='zx_mediaagency'/>
                                            <filter type='and'>
                                            <condition attribute='statecode' operator='eq' value='0'/>
                                            <condition attribute='zx_category' operator='eq' value='{((EntityReference)AnaplanOutput.Attributes["zx_category"]).Id.ToString()}' uitype='zx_category'/>
                                            </filter>
                                            </entity>
                                            </fetch>";

                    
                    EntityCollection entityCollection =  service.RetrieveMultiple(new FetchExpression(approvalrequest));

                    if(entityCollection.Entities.Count==0 && AnaplanOutput1.Contains("zx_brand"))
                    {
                        Entity budgetapproval= new Entity("zx_budgetapprovalrequest");
                            budgetapproval.Attributes["zx_category"] = getuserfromapprovalmatrix(service);
                            budgetapproval.Attributes["zx_marketinghead"] = getuserfromapprovalmatrix(service);
                            budgetapproval.Attributes["zx_mediaagency"] = getuserfromapprovalmatrix(service);
                            budgetapproval.Attributes["zx_centeralspoc"] = getuserfromapprovalmatrix(service);
                            budgetapproval.Attributes["zx_mhstatus"] = new OptionSetValue(128780001);
                        Guid data=service.Create(budgetapproval);

                        AnaplanOutput1.Attributes["zx_category"]= new EntityReference("zx_category", data);
                        service.Update(AnaplanOutput1);
                    }
                    else
                    {
                        if(AnaplanOutput1.Contains("zx_brand"))
                        {

                        AnaplanOutput1.Attributes["zx_request"] = new EntityReference("zx_budgetapprovalrequest", entityCollection.Entities[0].Id);
                        service.Update(AnaplanOutput1);
                        }
                    }

                }
             */   }
               
            }

        }


        private static EntityReference getuserfromapprovalmatrix(IOrganizationService service)
        {


            return new EntityReference("systemuser", new Guid(""));
        }


    }
}
