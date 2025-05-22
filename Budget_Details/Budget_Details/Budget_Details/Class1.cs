using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Details
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
                    Entity targetnew = (Entity)context.InputParameters["Target"];
                    Guid userid1 = context.UserId;
                    Entity budgetdetail = service.Retrieve(targetnew.LogicalName, targetnew.Id, new ColumnSet(true));
                    Guid zx_market = ((EntityReference)budgetdetail.Attributes["zx_market"]).Id;
                    Guid zx_anaplan = ((EntityReference)budgetdetail.Attributes["zx_anaplan"]).Id;

                    Entity loginuser = service.Retrieve("systemuser", userid1, new ColumnSet("zx_role"));
                    Guid roleid = ((EntityReference)loginuser.Attributes["zx_role"]).Id;
                    Entity role = service.Retrieve("zx_roles", roleid, new ColumnSet("zx_id"));

                    if (role.Contains("zx_id"))
                    {

                        int userloginrole = (int)role.Attributes["zx_id"];
                        Entity AP = service.Retrieve("zx_anaplanoutput", zx_anaplan, new ColumnSet(true));

                        switch (userloginrole)
                        {
                            case 0: // case 0 denotes the Category Head
                            sum(service, zx_market, zx_anaplan, "zx_challocation", "zx_challocation");
                              //  AP.Attributes["zx_challocation"] = getbudgetdata(service, "zx_challocation", zx_anaplan);
                                AP.Attributes["zx_challocationd"] = getbudgetdata(service, "zx_challocation", zx_anaplan);
                                service.Update(AP);

                                break;
                            case 1: // case 1 denotes the Marketing Head
                            sum(service, zx_market, zx_anaplan, "zx_mhallocation", "zx_mhallocation");
                             //   AP.Attributes["zx_mhallocation"] = getbudgetdata(service, "zx_mhallocation", zx_anaplan);
                                AP.Attributes["zx_mhallocationd"] = getbudgetdata(service, "zx_mhallocation", zx_anaplan).ToString();
                                service.Update(AP);
                                break;

                            case 2: // case 2 denotes the Brand Manager
                            sum(service, zx_market, zx_anaplan, "zx_bmallocation", "zx_bmallocation");
                                sum(service, zx_market, zx_anaplan, "zx_finalallocation", "zx_finalbudget");

                                //   AP.Attributes["zx_bmallocation"] = getbudgetdata(service, "zx_bmallocation", zx_anaplan);
                                AP.Attributes["zx_bmallocationd"] = getbudgetdata(service, "zx_bmallocation", zx_anaplan);
                             //   AP.Attributes["zx_finalbudgetd"] = getbudgetdata(service, "zx_finalbudget", zx_anaplan);

                                service.Update(AP);
                                break;

                            case 4: // case 4 denotes the CSPOC
                            sum(service, zx_market, zx_anaplan, "zx_finalallocation", "zx_finalbudget");
                             //   AP.Attributes["zx_finalbudget"] = getbudgetdata(service, "zx_finalbudget", zx_anaplan);
                                AP.Attributes["zx_finalbudgetd"] = getbudgetdata(service, "zx_finalbudget", zx_anaplan);
                                service.Update(AP);
                                break;
                            case 5: // case 2 denotes the Brand Manager or COE
                                sum(service, zx_market, zx_anaplan, "zx_bmallocation", "zx_bmallocation");
                                sum(service, zx_market, zx_anaplan, "zx_finalallocation", "zx_finalbudget");
                                //   AP.Attributes["zx_bmallocation"] = getbudgetdata(service, "zx_bmallocation", zx_anaplan);
                                AP.Attributes["zx_bmallocationd"] = getbudgetdata(service, "zx_bmallocation", zx_anaplan);
                              //  AP.Attributes["zx_finalbudgetd"] = getbudgetdata(service, "zx_finalbudget", zx_anaplan);

                                service.Update(AP);
                                break;
                            case 12: // case 2 denotes the Brand Manager or Centeral COE
                                sum(service, zx_market, zx_anaplan, "zx_finalallocation", "zx_finalbudget");
                                sum(service, zx_market, zx_anaplan, "zx_bmallocation", "zx_bmallocation");
                                //   AP.Attributes["zx_bmallocation"] = getbudgetdata(service, "zx_bmallocation", zx_anaplan);
                                AP.Attributes["zx_bmallocationd"] = getbudgetdata(service, "zx_bmallocation", zx_anaplan);
                            //    AP.Attributes["zx_finalbudgetd"] = getbudgetdata(service, "zx_finalbudget", zx_anaplan);

                                service.Update(AP);
                                break;



                        }

                    }


                    }


                }


            
        }
        private void sum(IOrganizationService service,Guid Market,Guid Anaplan,string fieldname, string fieldnamebudget)
        {
            decimal total = 0;

            string budgetdetailfetchxml = @"<fetch version='1.0' mapping='logical' savedqueryid='7394d20f-287f-476c-9539-23c425030439' no-lock='false' distinct='true'>" +
                                            "<entity name='zx_budgetdetail'>" +
                                            "<attribute name='zx_budgetdetailid'/>" +
                                            "<attribute name='zx_name'/>" +
                                            "<attribute name='zx_anaplan'/>" +
                                            "<attribute name='zx_budget'/>" +
                                            "<attribute name='zx_market'/>" +
                                            "<attribute name='zx_budgetacdmedium'/>" +
                                            "<attribute name='zx_finalallocation'/>" +
                                            "<attribute name='zx_tamweight'/>" +
                                            "<attribute name='zx_ctvytcohort'/>" +
                                            "<attribute name='zx_cpm'/>" +
                                            "<attribute name='zx_ctvcohort'/>" +
                                            "<order attribute='zx_name' descending='false'/>" +
                                            "<attribute name='zx_bmallocation'/>" +
                                            "<attribute name='zx_bmallocationp'/>" +
                                            "<attribute name='zx_challocation'/>" +
                                            "<attribute name='zx_challocationp'/>" +
                                            "<attribute name='zx_finalallocationp'/>" +
                                            "<attribute name='zx_mhallocation'/>" +
                                            "<attribute name='zx_mhallocationp'/>" +
                                            "<attribute name='zx_sumctvcohort'/>" +
                                            "<attribute name='zx_remarks'/>" +
                                            "<attribute name='zx_editexistingplan'/>" +
                                            "<attribute name='zx_reach'/>" +
                                            "<filter type='and'>" +
                                            "<condition attribute='statecode' operator='eq' value='0'/>" +
                                            "<filter type='and'>" +
                                            "<filter type='and'>" +
                                            "<condition attribute='zx_anaplan' operator='eq' value='{"+Anaplan.ToString()+"}'/>" +
                                            "</filter>" +
                                            "<filter type='and'>" +
                                            "<condition attribute='zx_market' operator='eq' value='{"+Market.ToString()+"}'/>" +
                                            "</filter>" +
                                            "</filter>" +
                                            "</filter>" +
                                            "</entity>" +
                                            "</fetch>";
            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(budgetdetailfetchxml));
            if(entityCollection.Entities.Count>0)
            {
                for (int i = 0; i < entityCollection.Entities.Count; i++)
                {
                    if (entityCollection.Entities[i].Contains(fieldname))
                    {
                    total = total + ((decimal)entityCollection.Entities[i][fieldname]);
                        
                    }

                }
                updateDigitaldata(service, Market,Anaplan,total, fieldnamebudget);
            }

        }

        private void updateDigitaldata(IOrganizationService service, Guid Market, Guid Anaplan,decimal allocationbudget,string fieldname)
        {
            Guid budgetmedium = getctvordigitalid(service, 6);

            string budget = "<fetch version='1.0' mapping='logical' savedqueryid='9f946183-1a3c-4142-bb14-271fe24c532d' no-lock='false' distinct='true'>" +
                            "<entity name='zx_budget'>" +
                            "<attribute name='zx_budgetid'/>" +
                            "<attribute name='zx_name'/>" +
                            "<order attribute='zx_name' descending='false'/>" +
                            "<attribute name='zx_market'/>" +
                            "<attribute name='zx_budget'/>" +
                            "<attribute name='zx_anaplan'/>" +
                            "<attribute name='zx_cpm'/>" +
                            "<attribute name='zx_tamweight'/>" +
                            "<attribute name='zx_ctvytcohort'/>" +
                            "<attribute name='zx_totalbudget'/>" +
                            "<attribute name='zx_budgetacdmedium'/>" +
                            "<attribute name='zx_ctvcohort'/>" +
                            "<attribute name='zx_bmallocation'/>" +
                            "<attribute name='zx_challocation'/>" +
                            "<attribute name='zx_editexistingplan'/>" +
                            "<attribute name='zx_finalbudget'/>" +
                            "<attribute name='zx_finalbudgetp'/>" +
                            "<attribute name='zx_mhallocation'/>" +
                            "<attribute name='zx_reach'/>" +
                            "<attribute name='zx_remarksdigital'/>" +
                            "<attribute name='zx_remarks'/>" +
                            "<attribute name='zx_sumctvcohort'/>" +
                            "<attribute name='zx_id'/>" +
                            "<filter type='and'>" +
                            "<condition attribute='statecode' operator='eq' value='0'/>" +
                            "<condition attribute='zx_anaplan' operator='eq' value='{"+Anaplan.ToString()+"}' uitype='zx_anaplanoutput'/>" +
                            "<condition attribute='zx_budgetacdmedium' operator='eq' value='{"+budgetmedium.ToString()+"}' uiname='Digital' uitype='zx_campaigntype'/>" +
                            "<condition attribute='zx_market' operator='eq' value='{"+Market.ToString()+"}' uitype='zx_markets'/>" +
                            "</filter>" +
                            "</entity>" +
                            "</fetch>";
            EntityCollection coll = service.RetrieveMultiple(new FetchExpression(budget));

            if (coll.Entities.Count == 1)
            {
                Entity Digital=service.Retrieve("zx_budget", coll.Entities[0].Id, new ColumnSet(true));
                Digital.Attributes[fieldname] = allocationbudget;
                service.Update(Digital);

            }

        }
        private static Guid getctvordigitalid(IOrganizationService service, int id)
        {
            string text = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n  <entity name='zx_campaigntype'>\r\n    <attribute name='zx_campaigntypeid' />\r\n    <attribute name='zx_name' />\r\n    <attribute name='createdon' />\r\n    <order attribute='zx_id' descending='false' />\r\n    <filter type='and'>\r\n      <condition attribute='zx_id' operator='eq' value='" + id + "' />\r\n    </filter>\r\n  </entity>\r\n</fetch>";
            EntityCollection val = service.RetrieveMultiple((QueryBase)new FetchExpression(text));
            return ((Collection<Entity>)(object)val.Entities)[0].Id;
        }


        private static decimal getbudgetdata(IOrganizationService service, string Fieldname, Guid anaplan)
        {
            Guid guid = getctvordigitalid(service,6);

            string text = @"<fetch version='1.0' mapping='logical' savedqueryid='9f946183-1a3c-4142-bb14-271fe24c532d' no-lock='false' distinct='true'>" +
                            "<entity name='zx_budget'>" +
                            "<attribute name='zx_budgetid'/>" +
                            "<attribute name='zx_name'/>" +
                            "<attribute name='zx_market'/>" +
                            "<attribute name='zx_budget'/>" +
                            "<attribute name='zx_anaplan'/>" +
                            "<attribute name='zx_cpm'/>" +
                            "<attribute name='zx_tamweight'/>" +
                            "<attribute name='zx_ctvytcohort'/>" +
                            "<attribute name='zx_totalbudget'/>" +
                            "<attribute name='zx_budgetacdmedium'/>" +
                            "<attribute name='zx_ctvcohort'/>" +
                            "<order attribute='zx_name' descending='false'/>" +
                            "<attribute name='zx_bmallocation'/>" +
                            "<attribute name='zx_challocation'/>" +
                            "<attribute name='zx_finalbudget'/>" +
                            "<attribute name='zx_finalbudgetp'/>" +
                            "<attribute name='zx_mhallocation'/>" +
                            "<attribute name='zx_sumctvcohort'/>" +
                            "<attribute name='createdby'/>" +
                            "<attribute name='zx_id'/>" +
                            "<attribute name='zx_editexistingplan'/>" +
                            "<attribute name='statuscode'/>" +
                            "<attribute name='zx_remarksdigital'/>" +
                            "<attribute name='zx_remarks'/>" +
                            "<attribute name='owningbusinessunit'/>" +
                            "<filter type='and'>" +
                            "<condition attribute='statecode' operator='eq' value='0'/>" +
                            "<filter type='and'>" +
                            "<condition attribute='zx_budgetacdmedium' operator='eq' value='{" + guid.ToString() + "}'/>" +
                            "<condition attribute='zx_anaplan' operator='eq' value='{" + anaplan.ToString() + "}' uitype='zx_anaplanoutput'/>" +
                            "</filter>" +
                            "</filter>" +
                            "</entity>" +
                            "</fetch>";
            EntityCollection val6 = service.RetrieveMultiple((QueryBase)new FetchExpression(text));
            decimal num = 0;

            foreach (Entity item in (Collection<Entity>)(object)val6.Entities)
            {
                if(item.Contains(Fieldname))
                {
                num += (decimal)((DataCollection<string, object>)(object)item.Attributes)[Fieldname];

                }
            }

            return num;


        }


    }
}


