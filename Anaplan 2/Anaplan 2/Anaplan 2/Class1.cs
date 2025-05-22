using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Anaplan_2
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

                    Entity AnaplanOutput1 = service.Retrieve("zx_anaplanoutput", AnaplanOutput.Id, new ColumnSet("zx_variant", "zx_brand", "zx_category", "zx_request", "zx_month", "zx_brandmanager"));
                    int zx_month = ((OptionSetValue)AnaplanOutput1.Attributes["zx_month"]).Value;
                    if (AnaplanOutput1.Contains("zx_variant"))
                    {
                        Guid variant = ((EntityReference)AnaplanOutput1.Attributes["zx_variant"]).Id;

                     
                        AnaplanOutput1.Attributes["zx_categoryhead"] = getuserfromapprovalmatrix(service, variant, 0);
                        AnaplanOutput1.Attributes["zx_marketinghead"] = getuserfromapprovalmatrix(service, variant, 1);
                        AnaplanOutput1.Attributes["zx_brandmanager"] = getuserfromapprovalmatrix(service, variant, 2);
                        AnaplanOutput1.Attributes["zx_mediaagency"] = getuserfromapprovalmatrix(service, variant, 3);
                        AnaplanOutput1.Attributes["zx_centeralspoc"] = getuserfromapprovalmatrix(service, variant, 4);
                        AnaplanOutput1.Attributes["zx_coe"] = getuserfromapprovalmatrix(service, variant, 5);
                        AnaplanOutput1.Attributes["zx_madisonplanner"] = getuserfromapprovalmatrix(service, variant, 9);
                        AnaplanOutput1.Attributes["zx_madisonaccounts"] = getuserfromapprovalmatrix(service, variant, 11);
                        AnaplanOutput1.Attributes["zx_digitalcoe"] = getuserfromapprovalmatrix(service, variant, 14);
                        AnaplanOutput1.Attributes["zx_centralcoe"] = getuserfromapprovalmatrix(service, variant, 12);
                        AnaplanOutput1.Attributes["zx_mediahead"] = getuserfromapprovalmatrix(service, variant, 8);





                        AnaplanOutput1.Attributes["zx_chstatus"] = new OptionSetValue(128780001);
                        service.Update(AnaplanOutput1);

                    }


                   /* if (AnaplanOutput1.Contains("zx_category"))
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
                                                    <condition attribute='zx_month' operator='eq' value='{zx_month}'/>
                                                    <condition attribute='createdon' operator='this-year'/>
                                                   <condition attribute='zx_category' operator='eq' value='{((EntityReference)AnaplanOutput1.Attributes["zx_category"]).Id.ToString()}' uitype='zx_category'/>
                                                   </filter>
                                                   </entity>
                                                   </fetch>";


                        EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(approvalrequest));

                        if (entityCollection.Entities.Count == 0 && AnaplanOutput1.Contains("zx_category"))
                        {
                            Entity budgetapproval = new Entity("zx_budgetapprovalrequest");


                            Guid cat = ((EntityReference)AnaplanOutput1.Attributes["zx_category"]).Id;

                            budgetapproval.Attributes["zx_category"] = AnaplanOutput1.Attributes["zx_category"];
                            budgetapproval.Attributes["zx_categoryhead"] = getuserfromapprovalmatrix(service, cat, 0);
                            budgetapproval.Attributes["zx_marketinghead"] = getuserfromapprovalmatrix(service, cat, 1);
                            budgetapproval.Attributes["zx_mediaagency"] = getuserfromapprovalmatrix(service, cat, 2);
                            budgetapproval.Attributes["zx_centeralspoc"] = getuserfromapprovalmatrix(service, cat, 3);
                            budgetapproval.Attributes["zx_chstatus"] = new OptionSetValue(128780001);
                            budgetapproval.Attributes["zx_month"] = new OptionSetValue(zx_month);
                            Guid data = service.Create(budgetapproval);

                            AnaplanOutput1.Attributes["zx_request"] = new EntityReference("zx_budgetapprovalrequest", data);
                            service.Update(AnaplanOutput1);
                            checkbrand(service, data, cat, zx_month);





                        }
                        else
                        {
                            if (AnaplanOutput1.Contains("zx_brand"))
                            {

                                if (entityCollection.Entities.Count > 0)
                                {
                                    AnaplanOutput1.Attributes["zx_request"] = new EntityReference("zx_budgetapprovalrequest", entityCollection.Entities[0].Id);
                                    service.Update(AnaplanOutput1);
                                    checkbrand(service, entityCollection.Entities[0].Id, ((EntityReference)entityCollection.Entities[0].Attributes["zx_category"]).Id, zx_month);

                                }
                            }
                        }

                    }
                    */
                }
            }
        }


        private static EntityReference getuserfromapprovalmatrix(IOrganizationService service, Guid variant, int role)
        {





            string roles1 = $@"<fetch version='1.0' mapping='logical' savedqueryid='57e4ede2-eff9-47cc-8698-f2548364c36b' no-lock='false' distinct='true'>
                            <entity name='zx_roles'>
                            <attribute name='zx_rolesid'/>
                            <attribute name='zx_name'/>
                            <attribute name='createdon'/>
                            <order attribute='zx_name' descending='false'/>
                            <attribute name='zx_id'/>
                            <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                            <condition attribute='zx_id' operator='eq' value='{role}'/>
                            </filter>
                            </entity>
                            </fetch>";
            EntityCollection entityCollection1 = service.RetrieveMultiple(new FetchExpression(roles1));




            string approvalmatrix = $@" <fetch version='1.0' output-format='xml-platform' mapping='logical' savedqueryid='56bb39a7-c30d-ef11-9f89-002248d4d4f0' no-lock='false' distinct='true'>
                                    <entity name='zx_approvalmatrix'>
                                    <attribute name='zx_name'/>
                                    <attribute name='zx_approvalmatrixid'/>
                                    <attribute name='zx_category'/>
                                    <attribute name='zx_role'/>
                                    <attribute name='statecode'/>
                                    <attribute name='zx_type'/>
                                    <attribute name='zx_user'/>
                                    <filter type='and'>
                                    <filter type='and'>
                                    <condition attribute='zx_variant' operator='eq' value='{variant.ToString()}'/>
                                    </filter>
                                    <filter type='and'>
                                    <condition attribute='zx_role' operator='eq' value='{entityCollection1.Entities[0].Id.ToString()}'/>
                                    </filter>
                                    <filter type='and'>
                                    <condition attribute='zx_type' operator='eq' value='128780000'/>
                                    </filter>
                                    </filter>
                                    </entity>
                                    </fetch>";
            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(approvalmatrix));
            if (entityCollection.Entities.Count == 0)
            {
                return null;
            }
            else
            {
                return new EntityReference("systemuser", ((EntityReference)entityCollection.Entities[0].Attributes["zx_user"]).Id);

            }
        }

        private static void checkbrand(IOrganizationService service, Guid budgetapproval,Guid cat, int month)
            {
            string anaplanoutput = $@"<fetch version='1.0' mapping='logical' savedqueryid='0980f71f-c206-4747-bc07-028a53af3c2a' no-lock='false' distinct='true'>
                                    <entity name='zx_anaplanoutput'>
                                    <attribute name='zx_anaplanoutputid'/>
                                    <attribute name='zx_name'/>
                                    <attribute name='zx_budgetacdmedium'/>
                                    <attribute name='zx_month'/>
                                    <attribute name='zx_totalbudget'/>
                                    <attribute name='zx_category'/>
                                    <attribute name='zx_brand'/>
                                    <attribute name='zx_variant'/>
                                    <order attribute='zx_name' descending='false'/>
                                    <filter type='and'>
                                    <condition attribute='statecode' operator='eq' value='0'/>
                                    <filter type='and'>
                                    <filter type='and'>
                                    <condition attribute='zx_month' operator='eq' value='{month}'/>
                                    </filter>
                                    <filter type='and'>
                                    <condition attribute='zx_category' operator='eq' value='{cat.ToString()}'/>
                                    </filter>
                                    </filter>
                                    </filter>
                                    </entity>
                                    </fetch>";

            EntityCollection budget = service.RetrieveMultiple(new FetchExpression(anaplanoutput));

            foreach (Entity record in budget.Entities)
            {
                Guid zx_brand = ((EntityReference)record.Attributes["zx_brand"]).Id;
                string documentdata = $@"<fetch version='1.0' mapping='logical' savedqueryid='d5917054-e2fa-4694-be78-80b17d83485b' no-lock='false' distinct='true'>
                                    <entity name='zx_documents'>
                                    <attribute name='zx_documentsid'/>
                                    <attribute name='zx_documentname'/>
                                    <attribute name='createdon'/>
                                    <order attribute='zx_documentname' descending='false'/>
                                    <attribute name='zx_brand'/>
                                    <attribute name='zx_request'/>
                                    <filter type='and'>
                                    <condition attribute='statecode' operator='eq' value='0'/>
                                    <condition attribute='zx_brand' operator='eq' value='{zx_brand.ToString()}'  uitype='zx_brand'/>
                                    <condition attribute='zx_request' operator='eq' value='{budgetapproval.ToString()}'  uitype='zx_budgetapprovalrequest'/>
                                    </filter>
                                    </entity>
                                    </fetch>";

             EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(documentdata));
                
                if(entityCollection.Entities.Count == 0 )
                {
                    Entity doc = new Entity("zx_documents");
                    doc.Attributes["zx_brand"] = record.Attributes["zx_brand"];
                    doc.Attributes["zx_request"] = new EntityReference("zx_budgetapprovalrequest", budgetapproval);

                    doc.Attributes["zx_documentname"] = record.FormattedValues["zx_brand"].ToString();

                    service.Create(doc);
                }



               


            }
        }

    }
}

