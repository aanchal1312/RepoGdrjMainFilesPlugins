using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPM_Email
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



                    Entity CPM1 = (Entity)context.InputParameters["Target"];
                    Guid userid1 = context.UserId;
                    Entity cpm = service.Retrieve(CPM1.LogicalName, CPM1.Id, new ColumnSet(true));
                    try
                    {
                       
                


                    string inputString = cpm.Attributes["zx_insertionorder"].ToString();
                    string zx_region = cpm.Attributes["zx_region"].ToString();
                    string zx_city = cpm.Attributes["zx_city"].ToString();
                    string zx_year_field_cpm_email = cpm.Attributes.Contains("zx_year") && cpm.Attributes["zx_year"] != null? cpm.Attributes["zx_year"].ToString(): string.Empty;

                        
                        int completeviews = 0;

                        //region start regionsplit
                        if (zx_region.Contains(","))
                        {
                            zx_region = zx_region.Split(',')[0];
                        }
                        //regionsplit start end

                        try
                        {
                            completeviews = Convert.ToInt32(cpm.Attributes["zx_completeviewsvideo"].ToString());
                        }
                        catch
                        {
                            completeviews = 0;
                        
                        }

                    Guid zx_cityguid=Guid.Empty;
                    int gettingdoublemonth = 1;


                    string[] splitString = inputString.Split('_');
                    string Country = splitString[0];
                    string variant = splitString[1];
                    //string month= splitString[2];
                    string month= cpm.Attributes.Contains("zx_month") && cpm.Attributes["zx_month"] != null ? cpm.Attributes["zx_month"].ToString() : string.Empty;
                    string Year= splitString[3];
                    string yt = splitString[4];
                    string camptype = splitString[5];
                    string Market= splitString[6];
                     

                    string monthvalue2 = string.Empty;
                    bool monthmapping = false;
                    if (month.Contains("-"))
                    {
                        string[] splitmonth= month.Split('-');
                        month = splitmonth[0];
                        monthvalue2 = splitmonth[1];
                        gettingdoublemonth = 2;
                        monthmapping = getctvordigitalid(service, "zx_monthmapping", month) != Guid.Empty && getctvordigitalid(service, "zx_monthmapping", monthvalue2) != Guid.Empty;
                    }
                    else
                    {
                        monthmapping = getctvordigitalid(service, "zx_monthmapping", month) != Guid.Empty;

                    }

                    bool variantyes = getctvordigitalid(service, "zx_variant", variant)!=Guid.Empty;
                    bool zx_marketsyes= getctvordigitalid(service, "zx_markets", zx_region) != Guid.Empty;
                    bool zx_cityyes = getctvordigitalid(service, "zx_city", zx_city) != Guid.Empty;
                    zx_cityguid = getctvordigitalid(service, "zx_city", zx_city);
                    // auto create city in the master on 09-09-2024 after last call discussion on friday i.e 06-09-2024 if their is ',' in a string split and get first record 


                    //start
                    if (zx_cityyes==false)
                    {
                       
                        if (zx_city.Contains(","))
                        {

                            string[] splitcity = zx_city.Split(',');
                            string cityname = splitcity[0];

                            if (searchcity(service, cityname) == Guid.Empty)
                            {
                                zx_cityguid = getnewcity(service, zx_city);
                                zx_cityyes=true;
                            }
                            else
                            {
                                zx_cityguid = searchcity(service, cityname);
                                zx_cityyes = true;
                            }
                        }
                        else
                        {
                            zx_cityguid = getnewcity(service, zx_city);
                            zx_cityyes = true;

                        }
                                                


                    }
                        // end


                        string compare = "zx_variant  " + ((variantyes) ?"Variant found" :"Variant Not Found in Master").ToString() + "\n"
                        + "region" + ((zx_marketsyes)?"Market Found ": "Market Not Found in Master").ToString()+"\n"
                    
                    + "zx_city" + ((zx_cityyes)?"City Found":"City not found in master").ToString() + "\n"
                    + "zx_monthmapping" + ((monthmapping)?"Month Found":"Month Not found in mapping").ToString() + "\n"
                    + "zx_year" + ((!string.IsNullOrEmpty(zx_year_field_cpm_email)) ? "Year Found" : "Year Not found".ToString());



                       
                        if (variantyes && zx_marketsyes && zx_cityyes && monthmapping)
                    {

                      //throw new InvalidPluginExecutionException(getctvordigitalid(service, "zx_variant", variant).ToString());
                      
                        Guid guid1 = getctvordigitalid(service, "zx_monthmapping", month);

                        Entity month1 = service.Retrieve("zx_monthmapping",guid1, new ColumnSet("zx_month"));
                        int monvalue = ((OptionSetValue)month1.Attributes["zx_month"]).Value;


                        Entity cpmdata = new Entity("zx_cpm");
                        cpmdata.Attributes["zx_insertionorderid"] = cpm.Attributes["zx_insertionorderid"].ToString();
                        cpmdata.Attributes["zx_brandname"] = new EntityReference("zx_variant", getctvordigitalid(service, "zx_variant", variant));
                        cpmdata.Attributes["zx_month"] = new OptionSetValue(monvalue);
                        cpmdata.Attributes["zx_youtube"] = yt.ToString();
                            
                        cpmdata.Attributes["zx_ctvmobilerural"] = new EntityReference("zx_campaigntype", getctvordigitalid(service, "zx_campaigntype", camptype));
                        cpmdata.Attributes["zx_impressions"] = (Convert.ToDecimal(cpm.Attributes["zx_impressions"]) > 0 && gettingdoublemonth == 2) ? (Convert.ToDecimal(cpm.Attributes["zx_impressions"]) / gettingdoublemonth) : Convert.ToDecimal(cpm.Attributes["zx_impressions"]);
                           
                            cpmdata.Attributes["zx_revenueadvcurrency"] = (Convert.ToDecimal(cpm.Attributes["zx_revenueadvcurrency"].ToString().Replace("Rs.", "")) > 0 && gettingdoublemonth == 2) ? (Convert.ToDecimal(cpm.Attributes["zx_revenueadvcurrency"].ToString().Replace("Rs.", "")) / gettingdoublemonth) : Convert.ToDecimal(cpm.Attributes["zx_revenueadvcurrency"].ToString().Replace("Rs.", ""));
                            
                            cpmdata.Attributes["zx_region"] = new EntityReference("zx_markets", getctvordigitalid(service, "zx_markets", zx_region));

                        cpmdata.Attributes["zx_city"] = new EntityReference("zx_city", zx_cityguid);
                           
                            cpmdata.Attributes["zx_completeviewsvideo"] = completeviews;
                        cpmdata.Attributes["zx_year"] = !string.IsNullOrEmpty(zx_year_field_cpm_email) && int.TryParse(zx_year_field_cpm_email, out int yearVal) ? (int?)yearVal : null;

                            if (monthvalue2!=string.Empty)
                        {
                            Guid guid2 = getctvordigitalid(service, "zx_monthmapping", monthvalue2);

                            Entity month2 = service.Retrieve("zx_monthmapping", guid2, new ColumnSet("zx_month"));
                            int monvalue1 = ((OptionSetValue)month2.Attributes["zx_month"]).Value;


                            Entity cpmdata1 = new Entity("zx_cpm");
                            cpmdata1.Attributes["zx_insertionorderid"] = cpm.Attributes["zx_insertionorderid"].ToString();
                            cpmdata1.Attributes["zx_brandname"] = new EntityReference("zx_variant", getctvordigitalid(service, "zx_variant", variant));
                            cpmdata1.Attributes["zx_month"] = new OptionSetValue(monvalue1);
                            cpmdata1.Attributes["zx_youtube"] = yt.ToString();
                            cpmdata1.Attributes["zx_ctvmobilerural"] = new EntityReference("zx_campaigntype", getctvordigitalid(service, "zx_campaigntype", camptype));
                            cpmdata1.Attributes["zx_impressions"] = (Convert.ToDecimal(cpm.Attributes["zx_impressions"])>0 && gettingdoublemonth==2) ? (Convert.ToDecimal(cpm.Attributes["zx_impressions"])/ gettingdoublemonth ): Convert.ToDecimal(cpm.Attributes["zx_impressions"]) ;
                            cpmdata1.Attributes["zx_revenueadvcurrency"] = (Convert.ToDecimal(cpm.Attributes["zx_revenueadvcurrency"].ToString().Replace("Rs.", "")) > 0 && gettingdoublemonth == 2) ? (Convert.ToDecimal(cpm.Attributes["zx_revenueadvcurrency"].ToString().Replace("Rs.", "")) / gettingdoublemonth) : Convert.ToDecimal(cpm.Attributes["zx_revenueadvcurrency"].ToString().Replace("Rs.", "")); ;
                            cpmdata1.Attributes["zx_region"] = new EntityReference("zx_markets", getctvordigitalid(service, "zx_markets", zx_region));
                            cpmdata1.Attributes["zx_city"] = new EntityReference("zx_city", zx_cityguid);
                            cpmdata1.Attributes["zx_completeviewsvideo"] = (completeviews>0) ? completeviews/2:completeviews;
                            cpmdata.Attributes["zx_year"] = !string.IsNullOrEmpty(zx_year_field_cpm_email) && int.TryParse(zx_year_field_cpm_email, out int yearVal2) ? (int?)yearVal2 : null;

                                if (true)
                            { 
                                service.Create(cpmdata1);
                            }

                        }

                        if (true)
                        {
                            service.Create(cpmdata);

                        }
                      //   service.Delete(cpm.LogicalName,cpm.Id);
                        cpm.Attributes["zx_remarks"] = "created";
                        service.Update(cpm);

                    }
                    else
                    {
                        cpm.Attributes["zx_remarks"] = compare;
                        service.Update(cpm);
                    }

                    }
                    catch (Exception e)
                    {
                        cpm.Attributes["zx_remarks"] = "No Insertion Order ID" + e;
                        service.Update(cpm);

                    }

                }



            }

        }
       private static Guid getctvordigitalid(IOrganizationService service, string id,string v1)
        {

            int no = 0;


            if(v1== "Mobile")
            {
                no = 3;
            }
            else if(v1== "Rural")
            {
                no = 2;
            }
            else if(v1=="CTV")
            {
                no = 0;
            }


            string finalstring= string.Empty;

            switch (id)
            {
                case "zx_variant":
                    finalstring = @"<fetch version='1.0' mapping='logical' savedqueryid='026bcb4d-2513-4035-ab8d-b5cbd5e16232' no-lock='false' distinct='true'>" +
                                  "<entity name='zx_variant'>" +
                                  "<attribute name='zx_variantid'/>" +
                                  "<attribute name='zx_variant'/>" +
                                  "<attribute name='createdon'/>" +
                                  "<order attribute='zx_variant' descending='false'/>" +
                                  "<attribute name='zx_anaplan'/>" +
                                  "<attribute name='zx_brand'/>" +
                                  "<attribute name='zx_emgcode'/>" +
                                  "<attribute name='zx_variants'/>" +
                                        "<attribute name='zx_emgdescriptioncpm'/>" +
                                  "<filter type='and'>" +
                                  "<condition attribute='statecode' operator='eq' value='0'/>" +
                                  "<condition attribute='zx_emgdescriptioncpm' operator='eq' value='" + v1.ToString()+"'/>" +
                                  "</filter>" +
                                  "</entity>" +
                                  "</fetch>";
                    break;
                case "zx_campaigntype":
                    finalstring = @"<fetch version='1.0' mapping='logical' savedqueryid='6c9ca7b0-5072-4fba-9a65-d276baede40e' no-lock='false' distinct='true'>" +
                         "<entity name='zx_campaigntype'>" +
                         "<attribute name='zx_campaigntypeid'/>" +
                         "<attribute name='zx_name'/>" +
                         "<attribute name='createdon'/>" +
                         "<order attribute='zx_name' descending='false'/>" +
                         "<filter type='and'>" +
                         "<condition attribute='statecode' operator='eq' value='0'/>" +
                         "<condition attribute='zx_id' operator='eq' value='"+no+ "'/>" +
                         "</filter>" +
                         "</entity>" +
                         "</fetch>";
                    break;
                case "zx_markets":
                    finalstring = @"<fetch version='1.0' mapping='logical' savedqueryid='2f739671-5005-4832-b139-5cc2e012bd21' no-lock='false' distinct='true'>" +
                        "<entity name='zx_markets'>" +
                        "<attribute name='zx_marketsid'/>" +
                        "<attribute name='zx_digitalmarkets'/>" +
                        "<order attribute='zx_digitalmarkets' descending='false'/>" +
                        "<attribute name='zx_mashmarkets'/>" +
                        "<attribute name='zx_module'/>" +
                        "<filter type='and'>" +
                        "<condition attribute='statecode' operator='eq' value='0'/>" +
                        "<condition attribute='zx_digitalmarkets' operator='eq' value='"+v1.ToString()+"'/>" +
                        "</filter>" +
                        "</entity>" +
                        "</fetch>";
                    break;
                case "zx_city":


                    finalstring = @"<fetch version='1.0' mapping='logical' savedqueryid='03328917-23d8-4d4d-9339-2f180fc40211' no-lock='false' distinct='true'>" +
                                "<entity name='zx_city'>" +
                                "<attribute name='zx_cityid'/>" +
                                "<attribute name='zx_name'/>" +
                                "<attribute name='createdon'/>" +
                                "<order attribute='zx_name' descending='false'/>" +
                                "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'/>" +
                                "<condition attribute='zx_name' operator='eq' value='"+v1.ToString()+"'/>" +
                                "</filter>" +
                                "</entity>" +
                                "</fetch>";
                    break;
                case "zx_monthmapping":
                    finalstring = @"<fetch version='1.0' mapping='logical' savedqueryid='ebfa180a-cfa3-47e0-a7e1-c5029076f86d' no-lock='false' distinct='true'>" +
                        "<entity name='zx_monthmapping'>" +
                        "<attribute name='zx_monthmappingid'/>" +
                        "<attribute name='zx_name'/>" +
                        "<attribute name='createdon'/>" +
                        "<order attribute='zx_name' descending='false'/>" +
                        "<attribute name='zx_month'/>" +
                        "<filter type='and'>" +
                        "<condition attribute='statecode' operator='eq' value='0'/>" +
                        "<condition attribute='zx_name' operator='eq' value='"+v1.ToString()+"'/>" +
                        "</filter>" +
                        "</entity>" +
                        "</fetch>";
                    break;



             
            }


            EntityCollection entityCollection1 = service.RetrieveMultiple(new FetchExpression(finalstring));
            Guid gid = Guid.Empty;
            if (entityCollection1.Entities.Count>0)
            {
             gid = entityCollection1.Entities[0].Id;
            }
            else if( entityCollection1.Entities.Count== 0 && id == "zx_variant")
            {
                finalstring = @"<fetch version='1.0' mapping='logical' savedqueryid='026bcb4d-2513-4035-ab8d-b5cbd5e16232' no-lock='false' distinct='true'>" +
              "<entity name='zx_variant'>" +
              "<attribute name='zx_variantid'/>" +
              "<attribute name='zx_variant'/>" +
              "<attribute name='createdon'/>" +
              "<order attribute='zx_variant' descending='false'/>" +
              "<attribute name='zx_anaplan'/>" +
              "<attribute name='zx_brand'/>" +
              "<attribute name='zx_emgcode'/>" +
              "<attribute name='zx_variants'/>" +
                    "<attribute name='zx_oldvariant'/>" +
              "<filter type='and'>" +
              "<condition attribute='statecode' operator='eq' value='0'/>" +
              "<condition attribute='zx_oldvariant' operator='eq' value='" + v1.ToString() + "'/>" +
              "</filter>" +
              "</entity>" +
              "</fetch>";
              EntityCollection entityCollectionNew = service.RetrieveMultiple(new FetchExpression(finalstring));
                if(entityCollectionNew.Entities.Count>0)
                {
                    gid= entityCollectionNew.Entities[0].Id;
                }



            }

            return gid;

        }


        private static int duplicatecheck(IOrganizationService service, string variant,string city, string region, int monnnn)
        {
         
            string fetchxml = @"<fetch version='1.0' mapping='logical' savedqueryid='62c41dc9-4378-4654-8c2d-864c3afea367' no-lock='false' distinct='true'>" +
                                "<entity name='zx_cpm'>" +
                                "<attribute name='zx_name'/>" +
                                "<attribute name='zx_cpmid'/>" +
                                "<attribute name='zx_brandname'/>" +
                                "<attribute name='zx_impressions'/>" +
                                "<attribute name='zx_revenueadvcurrency'/>" +
                                "<attribute name='zx_insertionorderid'/>" +
                                "<attribute name='zx_ctvmobilerural'/>" +
                                "<attribute name='zx_city'/>" +
                                "<attribute name='zx_cpm'/>" +
                                "<attribute name='zx_region'/>" +
                                "<attribute name='zx_month'/>" +
                                "<attribute name='createdon'/>" +
                                "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'/>" +
                                "<condition attribute='zx_brandname' operator='eq' value='{"+variant.ToString()+"}'  uitype='zx_variant'/>" +
                                "<condition attribute='zx_city' operator='eq' value='{"+city.ToString()+"}' uitype='zx_city'/>" +
                                "<condition attribute='zx_region' operator='eq' value='{"+region.ToString()+"}' uitype='zx_markets'/>" +
                                "<condition attribute='zx_month' operator='eq' value='"+ monnnn + "'/>" +
                                "<condition attribute='createdon' operator='this-year'/>" +
                                "</filter>" +
                                "</entity>" +
                                "</fetch>";
            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchxml));
            return entityCollection.Entities.Count;
        }
    
        private static Guid getnewcity(IOrganizationService service, string city_name)
        {

            Entity city=  new Entity("zx_city");
            city.Attributes["zx_name"] =city_name;
           Guid id=service.Create(city);

            return id;

        }


        private static Guid searchcity(IOrganizationService service, string city_)
        {
            string city = @"<fetch version='1.0' mapping='logical' savedqueryid='03328917-23d8-4d4d-9339-2f180fc40211' no-lock='false' distinct='true'>" +
                        "<entity name='zx_city'>" +
                        "<attribute name='zx_cityid'/>" +
                        "<attribute name='zx_name'/>" +
                        "<attribute name='createdon'/>" +
                        "<order attribute='zx_name' descending='false'/>" +
                        "<filter type='and'>" +
                        "<condition attribute='statecode' operator='eq' value='0'/>" +
                        "<condition attribute='zx_name' operator='like' value='%" + city_.ToString() + "%'/>" +
                        "</filter>" +
                        "</entity>" +
                        "</fetch>";

            EntityCollection collection = service.RetrieveMultiple(new FetchExpression(city));

            if(collection.Entities.Count > 0)
            {
                return collection.Entities[0].Id;
            }
            else
            {
                
                return Guid.Empty;

            }

        }
    }
}

