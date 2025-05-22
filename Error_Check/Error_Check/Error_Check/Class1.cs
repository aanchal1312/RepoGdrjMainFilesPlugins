using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error_Check
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

                    Guid userid1 = context.UserId;
                    Entity userid= service.Retrieve("systemuser", userid1, new ColumnSet(true));

                    int loginuserid = 0;


                    if(userid.Contains("zx_role"))
                    {
                        Guid userroleid =((EntityReference)userid.Attributes["zx_role"]).Id;
                   
                    Entity role = service.Retrieve("zx_roles", userroleid, new ColumnSet("zx_id"));
                        loginuserid = (int)role.Attributes["zx_id"];

                    }



                    Entity targetdata = (Entity)context.InputParameters["Target"];
                    Entity targetdata1 = service.Retrieve("zx_anaplanoutput", targetdata.Id, new ColumnSet(true));


                    int zx_bmstatus1 = ((OptionSetValue)targetdata1.Attributes["zx_bmstatus"]).Value;
                    int zx_centralspocstatus1 = ((OptionSetValue)targetdata1.Attributes["zx_centralspocstatus"]).Value;


                  
                        bool flag = (bool)targetdata1.Attributes["zx_editexistingplan"];
                    decimal zx_mhallocation = 0;
                    decimal zx_mhallocationd= 0;

                    string f1 = string.Empty;
                    string f2 = string.Empty;

                    decimal totalctv =0;
                    decimal totaldigital = 0;
                    Guid ctv = getctvordigitalid(service, 1);
                    Guid Digital = getctvordigitalid(service, 6);
                    Guid anaplandata = targetdata.Id;
                    if (flag== false)
                    {

                  
                   //((EntityReference)targetdata.Attributes["zx_anaplan"]).Id;
                    if (loginuserid==0)
                    {
                        f1 = "zx_challocation";
                        f2 = "zx_challocationd";
                        totalctv = getdata(service, ctv, anaplandata, "zx_challocation");
                        totaldigital = getdata(service, Digital, anaplandata, "zx_challocation");

                            //    if (totalctv > 0 && totaldigital > 0)

                            if (zx_bmstatus1 == 128780002)

                            {

                                targetdata1.Attributes["zx_challocation"] = totalctv;
                                targetdata1.Attributes["zx_challocationd"] = totaldigital;
                                service.Update(targetdata1);

                            }

                        

                        }
                    else if(loginuserid==1)
                    {
                        f1 = "zx_mhallocation";
                        f2 = "zx_mhallocationd";
                        totalctv = getdata(service, ctv, anaplandata, "zx_mhallocation");
                        totaldigital = getdata(service, Digital, anaplandata, "zx_mhallocation");

                            //    if (totalctv > 0 && totaldigital > 0)
                            if (zx_bmstatus1 == 128780002)
                            {

                                targetdata1.Attributes["zx_mhallocation"] = totalctv;
                                targetdata1.Attributes["zx_mhallocationd"] = totaldigital.ToString();
                                service.Update(targetdata1);

                            }
                        }
                    else if(loginuserid==2)
                    {
                        f1 = "zx_bmallocation";
                        f2 = "zx_bmallocationd";
                        totalctv = getdata(service, ctv, anaplandata, "zx_bmallocation");
                        totaldigital = getdata(service, Digital, anaplandata, "zx_bmallocation");


                            //     if(totalctv>0 && totaldigital>0)
                            if (zx_bmstatus1 == 128780002)
                            {

                              /*    targetdata1.Attributes["zx_finalbudget"] = totalctv;
                            targetdata1.Attributes["zx_finalbudgetd"] = totaldigital;*/

                                targetdata1.Attributes["zx_bmallocation"] = totalctv;
                                targetdata1.Attributes["zx_bmallocationd"] = totaldigital;

                                targetdata1.Attributes["zx_bmallocationp"] = totalctv;
                                targetdata1.Attributes["zx_bmallocationdp"] = totaldigital;


                                service.Update(targetdata1);

                            }

                        }
                    else if (loginuserid == 4)
                    {
                        f1 = "zx_finalbudget";
                        f2 = "zx_finalbudgetd";
                        totalctv = getdata(service, ctv, anaplandata, "zx_finalbudget");
                        totaldigital = getdata(service, Digital, anaplandata, "zx_finalbudget");
                            targetdata1.Attributes["zx_finalbudget"] = totalctv;
                            targetdata1.Attributes["zx_finalbudgetd"] = totaldigital;
                            service.Update(targetdata1);
                        }






                    }

                    else 
                    {
                       
                        if(flag==true)
                        {
                            if (loginuserid == 0)
                            {
                                f1 = "zx_challocation";
                                f2 = "zx_challocationd";
                                totalctv = getdata(service, ctv, anaplandata, "zx_challocation");
                                totaldigital = getdata(service, Digital, anaplandata, "zx_challocation");


                                targetdata1.Attributes["zx_challocation"] = totalctv;
                                targetdata1.Attributes["zx_challocationd"] = totaldigital;

                                targetdata1.Attributes["zx_mhallocation"] = totalctv;
                                targetdata1.Attributes["zx_mhallocationd"] = totaldigital.ToString();




                                targetdata1.Attributes["zx_finalbudget"] = totalctv;
                                targetdata1.Attributes["zx_finalbudgetd"] = totaldigital;
                                service.Update(targetdata1);
                            }
                            else if (loginuserid == 1)
                            {
                                f1 = "zx_mhallocation";
                                f2 = "zx_mhallocationd";
                                totalctv = getdata(service, ctv, anaplandata, "zx_mhallocation");
                                totaldigital = getdata(service, Digital, anaplandata, "zx_mhallocation");
                
                                targetdata1.Attributes["zx_mhallocation"] = totalctv;
                                targetdata1.Attributes["zx_mhallocationd"] = totaldigital.ToString();




                                targetdata1.Attributes["zx_finalbudget"] = totalctv;
                                targetdata1.Attributes["zx_finalbudgetd"] = totaldigital;
                                service.Update(targetdata1);
                            }
                         


                            else if (loginuserid == 4)
                            {
                                f1 = "zx_finalbudget";
                                f2 = "zx_finalbudgetd";
                                totalctv = getdata(service, ctv, anaplandata, "zx_finalbudget");
                                totaldigital = getdata(service, Digital, anaplandata, "zx_finalbudget");

                                targetdata1.Attributes["zx_finalbudget"] = totalctv;
                                targetdata1.Attributes["zx_finalbudgetd"] = totaldigital;
                                service.Update(targetdata1);
                            }

                            else if( loginuserid == 2)
                            {
                               decimal totalctv1 = getdata(service, ctv, anaplandata, "zx_bmallocation");
                                decimal totaldigital1 = getdata(service, Digital, anaplandata, "zx_bmallocation");
                                
                                
                                targetdata1.Attributes["zx_challocation"] = totalctv1;
                                targetdata1.Attributes["zx_challocationd"] = totaldigital1;

                                targetdata1.Attributes["zx_mhallocation"] = totalctv1;
                                targetdata1.Attributes["zx_mhallocationd"] = totaldigital1.ToString();




                                targetdata1.Attributes["zx_finalbudget"] = totalctv1;
                                targetdata1.Attributes["zx_finalbudgetd"] = totaldigital1;
                                service.Update(targetdata1);

                            }

                        }
                    }


                    if (targetdata1.Contains(f1) && targetdata1.Contains(f2))
                    {

                        zx_mhallocation = Convert.ToDecimal(targetdata1.Attributes[f1]);
                        zx_mhallocationd = Convert.ToDecimal(targetdata1.Attributes[f2]);


                    }

                    if (totalctv == zx_mhallocation && totaldigital==zx_mhallocationd)
                    {

                        return;
                    }
                    else
                    {

                        int zx_bmstatus = ((OptionSetValue)targetdata1.Attributes["zx_bmstatus"]).Value;
                        int zx_centralspocstatus = ((OptionSetValue)targetdata1.Attributes["zx_centralspocstatus"]).Value;


                        if (zx_bmstatus == 128780002 )
                        {
                        throw new InvalidPluginExecutionException(totalctv + " total ctv " + totaldigital + "Total Digital" +"Amount exceeds/lesser than the allocation on summary. Please correct here or on summary screen");

                        }
                     //   throw new Exception("Amount exceeds/lesser than the allocation on summary. Please correct here or on summary screen");
                  /*      throw new InvalidPluginExecutionException(totalctv.ToString() + "Amount exceeds/lesser than the allocation on summary. Please correct here or on summary screen"+
                            "MHallocation TV" + totalctv.ToString()+""+
                         "Mh allocation dig" + totaldigital.ToString());
                  */
                    }



                }

              
            }
           
        }
        private static decimal getdata(IOrganizationService service, Guid ctv1, Guid anaplandata,string fieldname)

        {

            string zx_budget = $@"<fetch version='1.0' mapping='logical' savedqueryid='9f946183-1a3c-4142-bb14-271fe24c532d' no-lock='false' distinct='true'>
                                        <entity name='zx_budget'>
                                        <attribute name='zx_budgetid'/>
                                        <attribute name='zx_name'/>
                                        <attribute name='zx_market'/>
                                        <attribute name='zx_budget'/>
                                        <attribute name='zx_anaplan'/>
                                        <attribute name='zx_cpm'/>
                                        <attribute name='zx_tamweight'/>
                                        <attribute name='zx_ctvcohort'/>
                                        <attribute name='zx_ctvytcohort'/>
                                        <attribute name='zx_totalbudget'/>
                                        <attribute name='{fieldname}'/>

                                        <attribute name='zx_budgetacdmedium'/>
                                        <order attribute='zx_name' descending='false'/>
                                        <filter type='and'>
                                        <condition attribute='statecode' operator='eq' value='0'/>
                                        <filter type='and'>
                                        <filter type='and'>
                                        <condition attribute='zx_budgetacdmedium' operator='eq' value='{ctv1.ToString()}'/>
                                        <condition attribute='zx_anaplan' operator='eq' value='{anaplandata.ToString()}' uitype='zx_anaplanoutput'/>
                                        </filter>
                                        </filter>
                                        </filter>
                                        </entity>
                                        </fetch>";

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(zx_budget));
            decimal total = 0m;

            for (int i = 0; i < entityCollection.Entities.Count; i++)
            {
                //Entity entity = service.Retrieve(i.log)
                // total = total + Convert.ToDecimal(entityCollection.Entities[i]["zx_finalbudget"]);
                if (entityCollection.Entities[i].Contains(fieldname))
                {
                total = total + Convert.ToDecimal(entityCollection.Entities[i].Attributes[fieldname]);

                }

            }

            return total;

        }


        private static Guid getctvordigitalid(IOrganizationService service,int id)
        {

            string zx_budgetdigital = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n  <entity name='zx_campaigntype'>\r\n    <attribute name='zx_campaigntypeid' />\r\n    <attribute name='zx_name' />\r\n    <attribute name='createdon' />\r\n    <order attribute='zx_id' descending='false' />\r\n    <filter type='and'>\r\n      <condition attribute='zx_id' operator='eq' value='" +id+"' />\r\n    </filter>\r\n  </entity>\r\n</fetch>";
            EntityCollection entityCollection1 = service.RetrieveMultiple(new FetchExpression(zx_budgetdigital));

            Guid gid = entityCollection1.Entities[0].Id;



            return gid;

        }


    }
}

