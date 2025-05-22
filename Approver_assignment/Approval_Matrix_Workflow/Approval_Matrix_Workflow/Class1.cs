using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Approval_Matrix_Workflow
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

                /*    if (context.MessageName == "Update")
                    {
                        AnaplanOutput = service.Retrieve("zx_approvalmatrix", AnaplanOutput.Id, new ColumnSet(true));
                    }*/
                    if (AnaplanOutput.Contains("zx_variant")
                        && AnaplanOutput.Contains("zx_user")
                        && AnaplanOutput.Contains("zx_role")
                        )
                    {
                        Guid roleguid = ((EntityReference)AnaplanOutput.Attributes["zx_role"]).Id;
                        Entity roleid = service.Retrieve("zx_roles", roleguid, new ColumnSet(true));
                        int role1 = (int)roleid.Attributes["zx_id"];
                        Guid userguid = ((EntityReference)AnaplanOutput.Attributes["zx_user"]).Id;
                        Guid variant = ((EntityReference)AnaplanOutput.Attributes["zx_variant"]).Id;
                      

                       
                        if(duplicatecheck(service,variant.ToString(),userguid.ToString(),roleguid.ToString()))
                        {

                        switch (role1)
                        {
                            case 0:
                                setapproverinanaplan(service, "zx_categoryhead", variant.ToString(), userguid);
                                break;
                            case 1:
                                setapproverinanaplan(service, "zx_marketinghead", variant.ToString(), userguid);
                                break;
                            case 2:
                                setapproverinanaplan(service, "zx_brandmanager", variant.ToString(), userguid);
                                break;
                            case 3:
                                setapproverinanaplan(service, "zx_mediaagency", variant.ToString(), userguid);
                                break;
                            case 4:
                                setapproverinanaplan(service, "zx_centeralspoc", variant.ToString(), userguid);
                                break;
                            case 9:
                                setapproverinanaplan(service, "zx_madisonplanner", variant.ToString(), userguid);
                                break;
                            case 11:
                                setapproverinanaplan(service, "zx_madisonaccounts", variant.ToString(), userguid);
                                break;
                            case 5:
                                setapproverinanaplan(service, "zx_coe", variant.ToString(), userguid);
                                break;
                            case 12:
                                setapproverinanaplan(service, "zx_centralcoe", variant.ToString(), userguid);
                                break;
                            case 14:
                                setapproverinanaplan(service, "zx_digitalcoe", variant.ToString(), userguid);
                                break;
                             case 8:
                                    setapproverinanaplan(service, "zx_mediahead", variant.ToString(), userguid);
                                    break;



                            }
                        }

                        else
                        {
                            throw new InvalidPluginExecutionException("Duplicate Record Found");
                        }
                        
                        

                    }

                }
            } }
        private static void setapproverinanaplan(IOrganizationService service, string ColumnlogicalName,string variant, Guid user)
        {


            string anaplanoutput7 = "<fetch version='1.0' mapping='logical' savedqueryid='0980f71f-c206-4747-bc07-028a53af3c2a' no-lock='false' distinct='true'>" +
                               "<entity name='zx_anaplanoutput'>" +
                               "<attribute name='statecode'/>" +
                               "<attribute name='zx_anaplanoutputid'/>" +
                               "<attribute name='zx_name'/>" +
                               "<order attribute='zx_name' descending='false'/>" +
                               "<attribute name='zx_budgetacdmedium'/>" +
                               "<attribute name='zx_month'/>" +
                               "<attribute name='zx_totalbudget'/>" +
                               "<attribute name='zx_category'/>" +
                               "<attribute name='zx_brand'/>" +
                               "<attribute name='zx_variant'/>" +
                               "<attribute name='zx_madisonplanner'/>" +
                               "<attribute name='zx_madisonaccounts'/>" +
                               "<attribute name='zx_marketinghead'/>" +
                               "<attribute name='zx_mediaagency'/>" +
                               "<attribute name='zx_centeralspoc'/>" +
                               "<attribute name='zx_categoryhead'/>" +
                               "<attribute name='zx_brandmanager'/>" +
                               "<attribute name='zx_coe'/>" +
                               "<filter type='and'>" +
                               "<condition attribute='statecode' operator='eq' value='0'/>" +
                               "<condition attribute='zx_variant' operator='eq' value='{"+variant.ToString()+"}' uitype='zx_variant'/>" +
                               "</filter>" +
                               "</entity>" +
                               "</fetch>";
            EntityCollection anaplancollection = service.RetrieveMultiple(new FetchExpression(anaplanoutput7));
            if(anaplancollection.Entities.Count>0)
            {
                for (int i = 0; i < anaplancollection.Entities.Count; i++)
                {
                    Entity   mash = service.Retrieve("zx_anaplanoutput", anaplancollection.Entities[i].Id, new ColumnSet(true));
                    mash.Attributes[ColumnlogicalName] = new EntityReference("systemuser", user);
                    service.Update(mash);
                }

            }
        
        }

        private static bool duplicatecheck(IOrganizationService service,string variant,string user,string role)
        {
            string approvalmatrix = "<fetch version='1.0' output-format='xml-platform' mapping='logical' savedqueryid='56bb39a7-c30d-ef11-9f89-002248d4d4f0' no-lock='false' distinct='true'>" +
                                    "<entity name='zx_approvalmatrix'>" +
                                    "<attribute name='zx_name'/>" +
                                    "<attribute name='zx_approvalmatrixid'/>" +
                                    "<attribute name='zx_role'/>" +
                                    "<attribute name='statecode'/>" +
                                    "<attribute name='zx_type'/>" +
                                    "<attribute name='zx_user'/>" +
                                    "<attribute name='zx_category'/>" +
                                    "<attribute name='zx_variant'/>" +
                                    "<filter type='and'>" +
                                    "<condition attribute='zx_role' operator='eq' value='{"+role.ToString()+"}' uitype='zx_roles'/>" +
                                    "<condition attribute='zx_variant' operator='eq' value='{"+variant.ToString()+"}' uitype='zx_variant'/>" +
                                
                                    "</filter>" +
                                    "</entity>" +
                                    "</fetch>";
            //    "<condition attribute='zx_user' operator='eq' value='{"+user.ToString()+"}' uitype='systemuser'/>" +
            EntityCollection col = service.RetrieveMultiple(new FetchExpression(approvalmatrix));
            if (col.Entities.Count > 0)
            {
                return false;

            }
            else
            {
                return true;

            }
 }

    }

}
