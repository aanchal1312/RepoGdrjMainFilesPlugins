using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Xml.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

//namespace SUM;

public class Class1 : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
             IPluginExecutionContext val = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        IOrganizationServiceFactory val2 = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        IOrganizationService val3 = val2.CreateOrganizationService((Guid?)((IExecutionContext)val).UserId);
        if (((IExecutionContext)val).Depth != 1 || !((DataCollection<string, object>)(object)((IExecutionContext)val).InputParameters).Contains("Target") || !(((DataCollection<string, object>)(object)((IExecutionContext)val).InputParameters)["Target"] is Entity))
        {
            return;
        }

        Entity val4 = (Entity)((DataCollection<string, object>)(object)((IExecutionContext)val).InputParameters)["Target"];

            Guid guid = getctvordigitalid(val3, 1);
            Guid guid2 = getctvordigitalid(val3, 6);
        
            Entity val5 = val3.Retrieve("zx_budget", val4.Id, new ColumnSet(true));
            Guid id = ((EntityReference)((DataCollection<string, object>)(object)val5.Attributes)["zx_anaplan"]).Id;
        if (val.MessageName=="Update")
        {
            Entity loginuser = val3.Retrieve("systemuser", (val).UserId,new ColumnSet("zx_role"));
            Guid roleid = ((EntityReference)loginuser.Attributes["zx_role"]).Id;
            Entity role = val3.Retrieve("zx_roles", roleid, new ColumnSet("zx_id"));
           
            if (role.Contains("zx_id"))
            {

            int userloginrole = (int)role.Attributes["zx_id"];

                    Entity AP = val3.Retrieve("zx_anaplanoutput", id, new ColumnSet(true));
            switch(userloginrole)
            {
                case 0:
                    AP.Attributes["zx_challocation"]= getbudgetdata(val3, "zx_challocation", guid, id);
                    AP.Attributes["zx_challocationd"] = getbudgetdata(val3, "zx_challocation", guid2, id);
                    val3.Update(AP);
                    break;
                case 1:
                    AP.Attributes["zx_mhallocation"] = getbudgetdata(val3, "zx_mhallocation", guid, id);
                    AP.Attributes["zx_mhallocationd"] = getbudgetdata(val3, "zx_mhallocation", guid2, id).ToString();
                    val3.Update(AP);
                    break;
                case 2:

                    AP.Attributes["zx_bmallocation"] = getbudgetdata(val3, "zx_bmallocation", guid, id);
                    AP.Attributes["zx_bmallocationd"] = getbudgetdata(val3, "zx_bmallocation", guid2, id);
                    val3.Update(AP);
                    break;
                     

                    case 4:
                    AP.Attributes["zx_finalbudget"] = getbudgetdata(val3, "zx_finalbudget", guid, id);
                    AP.Attributes["zx_finalbudgetd"] = getbudgetdata(val3, "zx_finalbudget", guid2, id);
                    val3.Update(AP);
                    break;
                case 12:
                    AP.Attributes["zx_bmallocation"] = getbudgetdata(val3, "zx_bmallocation", guid, id);
                    AP.Attributes["zx_bmallocationd"] = getbudgetdata(val3, "zx_bmallocation", guid2, id);
                    val3.Update(AP);
                    break;
                case 5:
                    AP.Attributes["zx_bmallocation"] = getbudgetdata(val3, "zx_bmallocation", guid, id);
                    AP.Attributes["zx_bmallocationd"] = getbudgetdata(val3, "zx_bmallocation", guid2, id);
                    val3.Update(AP);
                    break;

                }

            }
       
        }
        else
          if (val.MessageName == "Create")
        { 

        string text = "<fetch version='1.0' mapping='logical' savedqueryid='9f946183-1a3c-4142-bb14-271fe24c532d' no-lock='false' distinct='true'>\r\n                                    <entity name='zx_budget'>\r\n                                    <attribute name='zx_budgetid'/>\r\n                                    <attribute name='zx_name'/>\r\n                                    <order attribute='zx_name' descending='false'/>\r\n                                    <attribute name='zx_market'/>\r\n                                    <attribute name='zx_budget'/>\r\n                                    <attribute name='zx_anaplan'/>\r\n                                    <attribute name='zx_cpm'/>\r\n                                    <attribute name='zx_tamweight'/>\r\n                                    <attribute name='zx_ctvytcohort'/>\r\n                                    <attribute name='zx_totalbudget'/>\r\n                                    <attribute name='zx_budgetacdmedium'/>\r\n                                    <attribute name='zx_ctvcohort'/>\r\n                                    <filter type='and'>\r\n                                    <condition attribute='statecode' operator='eq' value='0'/>\r\n                                    <condition attribute='zx_budgetacdmedium' operator='eq' value='" + guid.ToString() + "' uitype='zx_campaigntype'/>\r\n                                    <condition attribute='zx_anaplan' operator='eq' value='" + id.ToString() + "' uitype='zx_anaplanoutput'/>\r\n                                    </filter>\r\n                                    </entity>\r\n                                    </fetch>";
        EntityCollection val6 = val3.RetrieveMultiple((QueryBase)new FetchExpression(text));
        decimal num = default(decimal);
        foreach (Entity item in (Collection<Entity>)(object)val6.Entities)
        {
            num += (decimal)((DataCollection<string, object>)(object)item.Attributes)["zx_budget"];
        }

        string text2 = "<fetch version='1.0' mapping='logical' savedqueryid='9f946183-1a3c-4142-bb14-271fe24c532d' no-lock='false' distinct='true'>\r\n                                    <entity name='zx_budget'>\r\n                                    <attribute name='zx_budgetid'/>\r\n                                    <attribute name='zx_name'/>\r\n                                    <order attribute='zx_name' descending='false'/>\r\n                                    <attribute name='zx_market'/>\r\n                                    <attribute name='zx_budget'/>\r\n                                    <attribute name='zx_anaplan'/>\r\n                                    <attribute name='zx_cpm'/>\r\n                                    <attribute name='zx_tamweight'/>\r\n                                    <attribute name='zx_ctvytcohort'/>\r\n                                    <attribute name='zx_totalbudget'/>\r\n                                    <attribute name='zx_budgetacdmedium'/>\r\n                                    <attribute name='zx_ctvcohort'/>\r\n                                    <filter type='and'>\r\n                                    <condition attribute='statecode' operator='eq' value='0'/>\r\n                                    <condition attribute='zx_budgetacdmedium' operator='eq' value='" + guid2.ToString() + "' uitype='zx_campaigntype'/>\r\n                                    <condition attribute='zx_anaplan' operator='eq' value='" + id.ToString() + "' uitype='zx_anaplanoutput'/>\r\n                                    </filter>\r\n                                    </entity>\r\n                                    </fetch>";
        EntityCollection val7 = val3.RetrieveMultiple((QueryBase)new FetchExpression(text2));
        decimal num2 = default(decimal);
        foreach (Entity item2 in (Collection<Entity>)(object)val7.Entities)
        {
            Entity val8 = val3.Retrieve("zx_budget", item2.Id, new ColumnSet(true));
            num2 += (decimal)((DataCollection<string, object>)(object)val8.Attributes)["zx_budget"];
        }

        Entity val9 = val3.Retrieve("zx_anaplanoutput", id, new ColumnSet(true));
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_totalbudget"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_challocation"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_finalbudget"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_challocationp"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_finalbudgetp"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_mhallocation"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_mhallocationp"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_bmallocation"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_bmallocationp"] = num;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_totalbudgetd"] = num2;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_challocationd"] = num2;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_finalbudgetd"] = num2;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_challocationdp"] = num2;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_finalbudgetdp"] = num2;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_mhallocationd"] = num2.ToString();
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_mhallocationdp"] = num2;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_bmallocationd"] = num2;
        ((DataCollection<string, object>)(object)val9.Attributes)["zx_bmallocationdp"] = num2;
      
        val3.Update(val9);
        }
    }

    private static Guid getctvordigitalid(IOrganizationService service, int id)
    {
         string text = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n  <entity name='zx_campaigntype'>\r\n    <attribute name='zx_campaigntypeid' />\r\n    <attribute name='zx_name' />\r\n    <attribute name='createdon' />\r\n    <order attribute='zx_id' descending='false' />\r\n    <filter type='and'>\r\n      <condition attribute='zx_id' operator='eq' value='" + id + "' />\r\n    </filter>\r\n  </entity>\r\n</fetch>";
        EntityCollection val = service.RetrieveMultiple((QueryBase)new FetchExpression(text));
        return ((Collection<Entity>)(object)val.Entities)[0].Id;
    }

    private static decimal getbudgetdata(IOrganizationService service,string Fieldname,Guid guid,Guid anaplan)
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
                        "<condition attribute='zx_budgetacdmedium' operator='eq' value='{"+guid.ToString()+"}'/>" +
                        "<condition attribute='zx_anaplan' operator='eq' value='{"+anaplan.ToString()+"}' uitype='zx_anaplanoutput'/>" +
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
