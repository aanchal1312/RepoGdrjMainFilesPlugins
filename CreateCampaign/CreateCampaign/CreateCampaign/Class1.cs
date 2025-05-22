
using Microsoft.SqlServer.Server;
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

namespace CreateCampaign
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
                    Guid zx_variant = ((EntityReference)anplanconfirm.Attributes["zx_variant"]).Id;
                    int status = ((OptionSetValue)anplanconfirm.Attributes["zx_centralspocstatus"]).Value;

                    //------------------------------------------------------------------------------------------
                    // Ensure values are retrieved only if the attributes exist; otherwise, default to 128780000
                    int ctv_creationstatus = anplanconfirm.Attributes.Contains("zx_campaignyoutubectvcreatedstatus")
                        ? ((OptionSetValue)anplanconfirm.Attributes["zx_campaignyoutubectvcreatedstatus"]).Value
                        : 128780000;

                    int mobile_creationstatus = anplanconfirm.Attributes.Contains("zx_campaignyoutubemobilecreatedstatus")
                        ? ((OptionSetValue)anplanconfirm.Attributes["zx_campaignyoutubemobilecreatedstatus"]).Value
                        : 128780000;

                    int rural_creationstatus = anplanconfirm.Attributes.Contains("zx_campaignyoutuberuralcreatedstatus")
                        ? ((OptionSetValue)anplanconfirm.Attributes["zx_campaignyoutuberuralcreatedstatus"]).Value
                        : 128780000;

                    int zx_coestatus = anplanconfirm.Attributes.Contains("zx_coestatus")
                        ? ((OptionSetValue)anplanconfirm.Attributes["zx_coestatus"]).Value
                        : 128780000;

                    // Main condition check
                    if (status == 128780002 && zx_coestatus == 128780000)
                    {
                        // CTV Campaign Creation
                        if (ctv_creationstatus == 128780001 && data(service, anplanconfirm.Id.ToString(), CT(service, 0).ToString()))
                        {
                            int campaignTypeCtv = anplanconfirm.Attributes.Contains("zx_campaignyoutubectvtype")
                                ? ((OptionSetValue)anplanconfirm.Attributes["zx_campaignyoutubectvtype"]).Value
                                : 0;

                            create(service, zx_variant, anplanconfirm, campaignTypeCtv, campaigntype(service, 0));
                            updatestatus(service, anplanconfirm.Id, "zx_campaignyoutubectvcreatedstatus");
                        }

                        // Rural Campaign Creation
                        if (rural_creationstatus == 128780001 && data(service, anplanconfirm.Id.ToString(), CT(service, 2).ToString()))
                        {
                            int campaignTypeRural = anplanconfirm.Attributes.Contains("zx_campaignyoutuberuraltype")
                                ? ((OptionSetValue)anplanconfirm.Attributes["zx_campaignyoutuberuraltype"]).Value
                                : 0;

                            create(service, zx_variant, anplanconfirm, campaignTypeRural, campaigntype(service, 2));
                            updatestatus(service, anplanconfirm.Id, "zx_campaignyoutuberuralcreatedstatus");
                        }

                        // Mobile Campaign Creation
                        if (mobile_creationstatus == 128780001 && data(service, anplanconfirm.Id.ToString(), CT(service, 3).ToString()))
                        {
                            int campaignCategoryMobile = anplanconfirm.Attributes.Contains("zx_campaigncategory")
                                ? ((OptionSetValue)anplanconfirm.Attributes["zx_campaigncategory"]).Value
                                : 0;

                            create(service, zx_variant, anplanconfirm, campaignCategoryMobile, campaigntype(service, 3));
                            updatestatus(service, anplanconfirm.Id, "zx_campaignyoutubemobilecreatedstatus");
                        }
                    


                    //---------------------------------------------------------------------




                }

            }



            }
        }


        public EntityReference zx_campaignobjective(IOrganizationService service)
        {
            string campaignobjective = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            <entity name='zx_campaignobjective'>
            <attribute name='zx_campaignobjectiveid'/>
            <attribute name='zx_name'/>
            <attribute name='createdon'/>
            <order attribute='zx_name' descending='false'/>
            <filter type='and'>
            <condition attribute='zx_campaignobjectiveids' operator='eq' value='0'/>
            </filter>
            </entity>
            </fetch>";
            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(campaignobjective));




            if (entityCollection.Entities.Count > 0)
            {
                Guid zx_campaignobjective = entityCollection.Entities[0].Id;
                return new EntityReference("zx_campaignobjective", zx_campaignobjective);
            }
            else
            {
                return null;
            }




        }



        public string zx_ad_format(IOrganizationService service, int no)
        {
            string adformat = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                            <entity name='zx_ad_format'>
                            <attribute name='zx_ad_formatid'/>
                            <attribute name='zx_format'/>
                            <attribute name='createdon'/>
                            <attribute name='zx_uid'/>
                            <order attribute='zx_format' descending='false'/>
                            <filter type='and'>
                            <condition attribute='zx_adformatid' operator='eq' value='1'/>
                            </filter>
                            </entity>
                            </fetch>";

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(adformat));
            string data = string.Empty;
            string data2 = string.Empty;
            if (entityCollection.Entities.Count > 0)
            {
                Guid zx_campaignobjective = entityCollection.Entities[0].Id;
                Entity zx_ad_format1 = service.Retrieve("zx_ad_format", zx_campaignobjective, new ColumnSet("zx_format"));
                data = "[{\"name\":\"" + zx_ad_format1.Attributes["zx_format"].ToString() + "\",\"id\":\"" + zx_campaignobjective.ToString() + "\"}]";
                data2 = zx_ad_format1.Attributes["zx_format"].ToString();
            }
            if (no == 1)
            {
                return data.ToString();

            }
            else if (no == 3) { return (entityCollection.Entities.Count > 0) ? entityCollection.Entities[0].Id.ToString() : ""; }
            else
            {
                return data2.ToString();

            }

        }


        public bool data(IOrganizationService service, string anpalanid, string budgetmedium)
        {
            string budgetdata = "<fetch version='1.0' mapping='logical' savedqueryid='7394d20f-287f-476c-9539-23c425030439' no-lock='false' distinct='true'>" +
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
                                "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'/>" +
                                "<filter type='and'>" +
                                "<filter type='and'>" +
                                "<condition attribute='zx_anaplan' operator='eq' value='{"+anpalanid.ToString()+"}'/>" +
                                "</filter>" +
                                "<filter type='and'>" +
                                "<condition attribute='zx_budgetacdmedium' operator='eq' value='{"+ budgetmedium .ToString()+ "}'/>" +
                                "</filter>" +
                                "<filter type='and'>" +
                                "<condition attribute='zx_finalallocation' operator='gt' value='0'/>" +
                                "</filter>" +
                                "</filter>" +
                                "</filter>" +
                                "</entity>" +
                                "</fetch>";

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(budgetdata));
            return entityCollection.Entities.Count>0;

        }

        public EntityReference campaigntype(IOrganizationService service, int id)
        {
            string campaigntype = "<fetch version='1.0' mapping='logical' savedqueryid='6c9ca7b0-5072-4fba-9a65-d276baede40e' no-lock='false' distinct='true'>" +
                                "<entity name='zx_campaigntype'>" +
                                "<attribute name='zx_campaigntypeid'/>" +
                                "<attribute name='zx_name'/>" +
                                "<attribute name='createdon'/>" +
                                "<order attribute='zx_name' descending='false'/>" +
                                "<attribute name='zx_id'/>" +
                                "<attribute name='statuscode'/>" +
                                "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'/>" +
                                "<condition attribute='zx_id' operator='eq' value='"+id+"'/>" +
                                "</filter>" +
                                "</entity>" +
                                "</fetch>";

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(campaigntype));

            if(entityCollection.Entities.Count>0) {

                return new EntityReference("zx_campaigntype", entityCollection.Entities[0].Id);
               
            }
            else
            {

            return null;
            }

        }
        public Guid CT(IOrganizationService service, int id)
        {
            string campaigntype = "<fetch version='1.0' mapping='logical' savedqueryid='6c9ca7b0-5072-4fba-9a65-d276baede40e' no-lock='false' distinct='true'>" +
                                "<entity name='zx_campaigntype'>" +
                                "<attribute name='zx_campaigntypeid'/>" +
                                "<attribute name='zx_name'/>" +
                                "<attribute name='createdon'/>" +
                                "<order attribute='zx_name' descending='false'/>" +
                                "<attribute name='zx_id'/>" +
                                "<attribute name='statuscode'/>" +
                                "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'/>" +
                                "<condition attribute='zx_id' operator='eq' value='" + id + "'/>" +
                                "</filter>" +
                                "</entity>" +
                                "</fetch>";

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(campaigntype));

            if (entityCollection.Entities.Count > 0)
            {

                return entityCollection.Entities[0].Id;

            }
            else
            {

                return Guid.Empty;
            }

        }


        public void create(IOrganizationService service,Guid zx_variant,Entity anplanconfirm,int camapaigncategorytype ,EntityReference budgetacidmedium)
        {
            Entity zx_variant1 = service.Retrieve("zx_variant", zx_variant, new ColumnSet(true));
            Entity campaign = new Entity("zx_campaign");
            campaign.Attributes["zx_variant"] = anplanconfirm.Attributes["zx_variant"];
            campaign.Attributes["zx_campaigntype"] = budgetacidmedium;  // anplanconfirm.Attributes["zx_budgetacdmedium"];
            campaign.Attributes["zx_name"] = zx_variant1.Attributes["zx_variant"].ToString() + "_Sustenance_YT";
            campaign.Attributes["zx_campaignobjective"] = zx_campaignobjective(service);
            campaign.Attributes["zx_country"] = "IN";
            campaign.Attributes["zx_ro"] = "1234";
            campaign.Attributes["zx_adformat"] = zx_ad_format(service, 1).ToString();
            campaign.Attributes["zx_adformattext"] = zx_ad_format(service, 2).ToString();
            campaign.Attributes["zx_adformats"] = new EntityReference("zx_ad_format", new Guid(zx_ad_format(service, 3)));



            //zx_coestatus


            //   var today = DateTime.Today;
            var today = ((DateTime)anplanconfirm.Attributes["zx_anaplandate"]);


            DateTime monthStart = new DateTime(today.Year, today.Month, 1);

           

            //var monthEnd = monthStart.AddMonths(2).AddDays(-1);---Prev code

            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            //   string mon = "1287800"+monthStart.ToString("MM");

            //var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthStart = monthStart;
            var lastMonthEnd = monthEnd.AddDays(-1);

            //campaign.Attributes["zx_campaignstartdate"] = monthStart.AddMonths(1);
            campaign.Attributes["zx_campaignstartdate"] = monthStart;
            campaign.Attributes["zx_anaplan"] = new EntityReference("zx_anaplanoutput", anplanconfirm.Id);
            campaign.Attributes["zx_campaignenddate"] = monthEnd;
            campaign.Attributes["zx_month"] = anplanconfirm.Attributes["zx_month"];
            campaign.Attributes["zx_newcampaigncategoryselected"] = new OptionSetValue(camapaigncategorytype);

            service.Create(campaign);
          


        }
        
        public void updatestatus(IOrganizationService service, Guid anaplan,string field)
        {
            Entity updateAnaplan = new Entity("zx_anaplanoutput", anaplan);
            updateAnaplan[field] = new OptionSetValue(128780002);

            service.Update(updateAnaplan);
        }
        
    }


}

