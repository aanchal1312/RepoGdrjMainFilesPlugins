using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ResetForm
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
                    Entity anplanconfirm = (Entity)context.InputParameters["Target"];

                    anplanconfirm = service.Retrieve("zx_anaplanoutput", anplanconfirm.Id, new ColumnSet(true));

                    Guid guid1 = getctvordigitalid(service, 1);
                    Guid guid = getctvordigitalid(service, 6);




                    if ((bool)anplanconfirm.Attributes["zx_resetform"])

                      {

                    anplanconfirm.Attributes["zx_chstatus"] = new OptionSetValue(128780001);
                    anplanconfirm.Attributes["zx_bmstatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_mhstatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_centralspocstatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_coestatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_prestatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_provplanstatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_estimatesstatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_invoicestatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_buyinggridstatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_editexistingplan"] =false;
                    anplanconfirm.Attributes["zx_documentstatus"] = new OptionSetValue(128780000);
                    anplanconfirm.Attributes["zx_resetform"] = false;
                        Guid anaplanid = anplanconfirm.Id;
                     





                        string fetchxmlbudget = @"<fetch version='1.0' mapping='logical' savedqueryid='9f946183-1a3c-4142-bb14-271fe24c532d' no-lock='false' distinct='true'>" +
                        "<entity name='zx_budget'>" +
                        "<attribute name='statecode'/>" +
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
                        "<attribute name='zx_finalbudget'/>" +
                        "<attribute name='zx_finalbudgetp'/>" +
                        "<attribute name='zx_editexistingplan'/>" +
                        "<attribute name='zx_id'/>" +
                        "<attribute name='zx_mhallocation'/>" +
                        "<attribute name='zx_sumctvcohort'/>" +
                        "<attribute name='zx_remarksdigital'/>" +
                        "<attribute name='zx_remarks'/>" +
                        "<attribute name='zx_reach'/>" +
                        "<filter type='and'>" +
                        "<condition attribute='statecode' operator='eq' value='0'/>" +
                        "<condition attribute='zx_anaplan' operator='eq' value='{"+ anaplanid.ToString()+"}' uitype='zx_anaplanoutput'/>" +
                        "</filter>" +
                        "</entity>" +
                        "</fetch>";

                        EntityCollection  budgetcount= service.RetrieveMultiple(new FetchExpression(fetchxmlbudget));

                        foreach (Entity record in budgetcount.Entities)
                        {
                           Entity budgetnew=service.Retrieve("zx_budget",record.Id,new ColumnSet("zx_budget", "zx_challocation", "zx_bmallocation", "zx_mhallocation", "zx_finalbudget", "zx_totalbudget"));
                            decimal bd = 0;
                            if(budgetnew.Contains("zx_budget"))
                            {
                                bd = (decimal)budgetnew.Attributes["zx_budget"];
                            }
                            budgetnew.Attributes["zx_challocation"] = bd;
                            budgetnew.Attributes["zx_bmallocation"] = bd;
                            budgetnew.Attributes["zx_mhallocation"] = bd;
                            budgetnew.Attributes["zx_finalbudget"] = bd;
                            budgetnew.Attributes["zx_totalbudget"] = bd;
                            service.Update(budgetnew);  
                        }


                        string Budget_details = @"<fetch version='1.0' mapping='logical' savedqueryid='7394d20f-287f-476c-9539-23c425030439' no-lock='false' distinct='true'>" +
                            "<entity name='zx_budgetdetail'>" +
                            "<attribute name='zx_budgetdetailid'/>" +
                            "<attribute name='zx_name'/>" +
                            "<order attribute='zx_name' descending='false'/>" +
                            "<attribute name='zx_anaplan'/>" +
                            "<attribute name='zx_budget'/>" +
                            "<attribute name='zx_market'/>" +
                            "<attribute name='zx_budgetacdmedium'/>" +
                            "<attribute name='zx_finalallocation'/>" +
                            "<attribute name='zx_tamweight'/>" +
                            "<attribute name='zx_ctvytcohort'/>" +
                            "<attribute name='zx_cpm'/>" +
                            "<attribute name='zx_ctvcohort'/>" +
                            "<attribute name='zx_bmallocation'/>" +
                            "<attribute name='zx_bmallocationp'/>" +
                            "<attribute name='zx_challocation'/>" +
                            "<attribute name='zx_challocationp'/>" +
                            "<attribute name='zx_finalallocationp'/>" +
                            "<attribute name='zx_mhallocation'/>" +
                            "<attribute name='zx_mhallocationp'/>" +
                            "<attribute name='zx_reach'/>" +
                            "<attribute name='zx_sumctvcohort'/>" +
                            "<filter type='and'>" +
                            "<condition attribute='statecode' operator='eq' value='0'/>" +
                            "<condition attribute='zx_anaplan' operator='eq' value='{"+ anaplanid.ToString()+"}' uitype='zx_anaplanoutput'/>" +
                            "</filter>" +
                            "</entity>" +
                            "</fetch>";
                        EntityCollection budgetcount1 = service.RetrieveMultiple(new FetchExpression(Budget_details));

                        foreach (Entity record in budgetcount1.Entities)
                        {
                            Entity budgetnew = service.Retrieve("zx_budgetdetail", record.Id, new ColumnSet("zx_budget", "zx_challocationp",
                            "zx_challocation",
                            "zx_mhallocation",
                            "zx_mhallocationp",
                            "zx_bmallocation",
                            "zx_bmallocationp",
                            "zx_finalallocation",
                            "zx_finalallocationp"));
                            decimal bd = 0;
                            if (budgetnew.Contains("zx_budget"))
                            {
                                bd = (decimal)budgetnew.Attributes["zx_budget"];
                            }
                            budgetnew.Attributes["zx_challocationp"] = bd;
                            budgetnew.Attributes["zx_challocation"] = bd;
                            budgetnew.Attributes["zx_mhallocation"] = bd;
                            budgetnew.Attributes["zx_mhallocationp"] = bd;
                            budgetnew.Attributes["zx_bmallocation"] = bd;
                            budgetnew.Attributes["zx_bmallocationp"] = bd;
                            budgetnew.Attributes["zx_finalallocation"] = bd;
                            budgetnew.Attributes["zx_finalallocationp"] = bd;
                            service.Update(budgetnew);
                        }

                        decimal digitalvalue = getbudgetdata(service, "zx_budget", guid, anaplanid);
                        anplanconfirm.Attributes["zx_challocationd"] = digitalvalue;
                        anplanconfirm.Attributes["zx_challocationdp"] = digitalvalue;
                        anplanconfirm.Attributes["zx_mhallocationd"] = digitalvalue.ToString();
                        anplanconfirm.Attributes["zx_mhallocationdp"] = digitalvalue;
                        anplanconfirm.Attributes["zx_totalbudgetd"] = digitalvalue;
                        anplanconfirm.Attributes["zx_finalbudgetd"] = digitalvalue;
                        anplanconfirm.Attributes["zx_finalbudgetdp"] = digitalvalue;
                        anplanconfirm.Attributes["zx_bmallocationd"] = digitalvalue;
                        anplanconfirm.Attributes["zx_mhallocationdp"] = digitalvalue;
                        anplanconfirm.Attributes["zx_bmallocationdp"] = digitalvalue;

                        decimal tv = getbudgetdata(service, "zx_budget", guid1, anaplanid);

                        anplanconfirm.Attributes["zx_challocation"] = tv;
                        anplanconfirm.Attributes["zx_challocationp"] = tv;
                        anplanconfirm.Attributes["zx_mhallocation"] = tv;
                        anplanconfirm.Attributes["zx_mhallocationp"] = tv;
                        anplanconfirm.Attributes["zx_totalbudget"] = tv;
                        anplanconfirm.Attributes["zx_finalbudget"] = tv;
                        anplanconfirm.Attributes["zx_finalbudgetp"] = tv;
                        anplanconfirm.Attributes["zx_bmallocation"] = tv;
                        anplanconfirm.Attributes["zx_bmallocationp"] = tv;


                        








                    }

                   

                    service.Update(anplanconfirm);


                }

            }
            }

        private static Guid getctvordigitalid(IOrganizationService service, int id)
        {
            string text = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n  <entity name='zx_campaigntype'>\r\n    <attribute name='zx_campaigntypeid' />\r\n    <attribute name='zx_name' />\r\n    <attribute name='createdon' />\r\n    <order attribute='zx_id' descending='false' />\r\n    <filter type='and'>\r\n      <condition attribute='zx_id' operator='eq' value='" + id + "' />\r\n    </filter>\r\n  </entity>\r\n</fetch>";
            EntityCollection val = service.RetrieveMultiple((QueryBase)new FetchExpression(text));
            return ((Collection<Entity>)(object)val.Entities)[0].Id;
        }

        private static decimal getbudgetdata(IOrganizationService service, string Fieldname, Guid guid, Guid anaplan)
        {


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
                if (item.Contains(Fieldname))
                {

                    num += (decimal)((DataCollection<string, object>)(object)item.Attributes)[Fieldname];
                }
            }

            return num;


        }

    }


      


    }




