using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Rolling_Forecast_Upload
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
                    Entity tg = (Entity)context.InputParameters["Target"];

                    Entity rf= service.Retrieve(tg.LogicalName, tg.Id, new ColumnSet(true));
                    if(rf.Contains("zx_month") && rf.Contains("zx_variant"))
                    {
                    int month = ((OptionSetValue)rf.Attributes["zx_month"]).Value;
                    Guid variant = ((EntityReference)rf.Attributes["zx_variant"]).Id;
                    Guid anaplanid=getuser(service, variant, month);
                         if(anaplanid !=Guid.Empty)
                        {
                        Entity ap=service.Retrieve("zx_anaplanoutput", anaplanid, new ColumnSet(true));
                            ap.Attributes["zx_rollingforecast"] = rf.Attributes["zx_tv"];
                            ap.Attributes["zx_rollingforecastd"] = rf.Attributes["zx_digitalmarketing"];
                            service.Update(ap);
                        }



                    }

                   



                }

            }
        }

        public static Guid getuser(IOrganizationService service, Guid variant, int month)
        {
            string anaplanoutput = $@"<fetch version='1.0' mapping='logical' savedqueryid='0980f71f-c206-4747-bc07-028a53af3c2a' no-lock='false' distinct='true'>
<entity name='zx_anaplanoutput'>
<attribute name='zx_anaplanoutputid'/>
<attribute name='zx_name'/>
<order attribute='zx_name' descending='false'/>
<attribute name='zx_budgetacdmedium'/>
<attribute name='zx_month'/>
<attribute name='zx_totalbudget'/>
<attribute name='zx_category'/>
<attribute name='zx_brand'/>
<attribute name='zx_variant'/>
<filter type='and'>
<condition attribute='statecode' operator='eq' value='0'/>
<condition attribute='zx_month' operator='eq' value='{month}'/>
<condition attribute='zx_variant' operator='eq' value='{variant.ToString()}' uitype='zx_variant'/>
</filter>
</entity>
</fetch>";

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(anaplanoutput));
            if (entityCollection.Entities.Count > 0)
            {
                Guid anaplan = entityCollection.Entities[0].Id;
                return anaplan;

            }
            else
            {
                return Guid.Empty;
            }





        }
    }
   
}

