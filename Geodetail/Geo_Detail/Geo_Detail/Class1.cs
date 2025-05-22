//6 all set -only not available is English language and creative creation if the Geoenglish universal field has data at end. and similary if hindi universal field has then creating Hindi
//OLD
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Geo_Detail
{
    public class Languagenew
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.Depth == 1)
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity bd = (Entity)context.InputParameters["Target"];
                    try
                    {
                        bd = service.Retrieve("zx_geodetails", bd.Id, new ColumnSet(true));
                        if (!bd.Contains("zx_campaign"))
                        {
                            return;
                        }

                        Entity campaign = service.Retrieve("zx_campaign", ((EntityReference)bd["zx_campaign"]).Id,
                            new ColumnSet("zx_campaigntype", "zx_variant", "zx_newcampaigncategoryselected"));
                        int camp_selected = campaign.Contains("zx_newcampaigncategoryselected") ?
                            ((OptionSetValue)campaign["zx_newcampaigncategoryselected"]).Value : 0;

                        if (camp_selected != 128780002)
                        {
                            Guid zx_campaigntype = campaign.Contains("zx_campaigntype") ?
                                ((EntityReference)campaign["zx_campaigntype"]).Id : Guid.Empty;
                            Entity campaignTypeRecord = service.Retrieve("zx_campaigntype", zx_campaigntype, new ColumnSet("zx_id"));
                            int zx_id = campaignTypeRecord.Contains("zx_id") ? Convert.ToInt32(campaignTypeRecord["zx_id"]) : 0;

                            string marketname = bd.Contains("zx_name") ? bd["zx_name"].ToString().Trim() : string.Empty;
                            if (string.IsNullOrEmpty(marketname))
                            {
                                return;
                            }

                            List<string> processedMarkets = ProcessMarketNames(service, marketname);
                            if (processedMarkets.Count == 0)
                            {
                                throw new InvalidPluginExecutionException($"No valid markets processed from input: {marketname}");
                            }

                            regionallanguageandlanguagecreationmethod(service, context, processedMarkets, marketname, bd, campaign, camp_selected, zx_id);
                        }

                        List<string> processedMarkets2 = ProcessMarketNames(service, bd["zx_name"].ToString());


                        Guid campaignTypeId2 = campaign.Contains("zx_campaigntype") ? ((EntityReference)campaign["zx_campaigntype"]).Id : Guid.Empty;
                        int totalUniverseSize = CalculateUniverseSize(service, processedMarkets2, campaignTypeId2);

                        if (totalUniverseSize > 0)
                        {
                            bd["zx_universesize"] = totalUniverseSize;
                            service.Update(bd);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException($"Error in Execute: {ex.Message}", ex);
                    }
                }
            }
        }


        private List<string> ProcessMarketNames(IOrganizationService service, string marketnameInput)
        {
            if (string.IsNullOrWhiteSpace(marketnameInput))
                return new List<string>();

            List<string> processedMarkets = new List<string>();
            // Split input by '+' and trim whitespace
            string[] marketArray = marketnameInput.Split('+').Select(m => m.Trim()).ToArray();
            HashSet<string> processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Track processed markets

            // Step 1: Check if the entire input matches a single market entry
            string fetchXmlEntire = $@"
        <fetch version='1.0' mapping='logical' distinct='true'>
            <entity name='zx_markets'>
                <attribute name='zx_marketsid'/>
                <attribute name='zx_digitalmarkets'/>
                <attribute name='zx_digitalmarketmapping'/>
                <filter type='and'>
                    <condition attribute='statecode' operator='eq' value='0'/>
                    <filter type='or'>
                        <condition attribute='zx_digitalmarkets' operator='eq' value='{marketnameInput}'/>
                        <condition attribute='zx_digitalmarketmapping' operator='eq' value='{marketnameInput}'/>
                    </filter>
                </filter>
            </entity>
        </fetch>";

            EntityCollection entireResult = service.RetrieveMultiple(new FetchExpression(fetchXmlEntire));
            if (entireResult.Entities.Count > 0)
            {
                var entity = entireResult.Entities[0];
                string digitalMarket = entity.Contains("zx_digitalmarketmapping") && !string.IsNullOrEmpty(entity["zx_digitalmarketmapping"].ToString())
                    ? entity["zx_digitalmarketmapping"].ToString()
                    : entity["zx_digitalmarkets"].ToString();
                processedMarkets.Add(digitalMarket);
                return processedMarkets;
            }

            // Step 2: Process markets, checking for concatenated pairs first
            for (int i = 0; i < marketArray.Length; i++)
            {
                if (processed.Contains(marketArray[i]))
                    continue;

                bool marketProcessed = false;

                // Try concatenated market (pair) if not at the last market
                if (i < marketArray.Length - 1)
                {
                    string concatMarket = $"{marketArray[i]}+{marketArray[i + 1]}";
                    // Try both original case and uppercase to handle case sensitivity
                    string[] concatVariants = new[] { concatMarket, concatMarket.ToUpper() };

                    foreach (string variant in concatVariants)
                    {
                        string fetchXmlConcat = $@"
                    <fetch version='1.0' mapping='logical' distinct='true'>
                        <entity name='zx_markets'>
                            <attribute name='zx_marketsid'/>
                            <attribute name='zx_digitalmarkets'/>
                            <attribute name='zx_digitalmarketmapping'/>
                            <filter type='and'>
                                <condition attribute='statecode' operator='eq' value='0'/>
                                <filter type='or'>
                                    <condition attribute='zx_digitalmarkets' operator='eq' value='{variant}'/>
                                    <condition attribute='zx_digitalmarketmapping' operator='eq' value='{variant}'/>
                                </filter>
                            </filter>
                        </entity>
                    </fetch>";

                        EntityCollection concatResult = service.RetrieveMultiple(new FetchExpression(fetchXmlConcat));
                        if (concatResult.Entities.Count > 0)
                        {
                            var entity = concatResult.Entities[0];
                            string digitalMarket = entity.Contains("zx_digitalmarketmapping") && !string.IsNullOrEmpty(entity["zx_digitalmarketmapping"].ToString())
                                ? entity["zx_digitalmarketmapping"].ToString()
                                : entity["zx_digitalmarkets"].ToString();
                            processedMarkets.Add(digitalMarket);
                            processed.Add(marketArray[i]);
                            processed.Add(marketArray[i + 1]);
                            i++; // Skip the next market
                            marketProcessed = true;
                            break;
                        }
                    }
                }

                // Step 3: Process individual market if no concatenated market was found
                if (!marketProcessed)
                {
                    string currentMarket = marketArray[i];

                    // Step 4: Check for underscore (e.g., Bihar_Jharkhand)
                    if (currentMarket.Contains("_"))
                    {
                        string replacedMarket = GetConcatenatedMarketFromCRM(service, currentMarket);
                        if (!string.IsNullOrEmpty(replacedMarket) && replacedMarket != currentMarket)
                        {
                            processedMarkets.Add(replacedMarket);
                            processed.Add(currentMarket);
                            continue;
                        }
                    }

                    // Try both original case and uppercase for individual market
                    string[] marketVariants = new[] { currentMarket, currentMarket.ToUpper() };

                    foreach (string variant in marketVariants)
                    {
                        string fetchXmlIndividual = $@"
                    <fetch version='1.0' mapping='logical' distinct='true'>
                        <entity name='zx_markets'>
                            <attribute name='zx_marketsid'/>
                            <attribute name='zx_digitalmarkets'/>
                            <attribute name='zx_digitalmarketmapping'/>
                            <filter type='and'>
                                <condition attribute='statecode' operator='eq' value='0'/>
                                <filter type='or'>
                                    <condition attribute='zx_digitalmarkets' operator='eq' value='{variant}'/>
                                    <condition attribute='zx_digitalmarketmapping' operator='eq' value='{variant}'/>
                                </filter>
                            </filter>
                        </entity>
                    </fetch>";

                        EntityCollection individualResult = service.RetrieveMultiple(new FetchExpression(fetchXmlIndividual));
                        if (individualResult.Entities.Count > 0)
                        {
                            var entity = individualResult.Entities[0];
                            string digitalMarket = entity.Contains("zx_digitalmarketmapping") && !string.IsNullOrEmpty(entity["zx_digitalmarketmapping"].ToString())
                                ? entity["zx_digitalmarketmapping"].ToString()
                                : entity["zx_digitalmarkets"].ToString();
                            processedMarkets.Add(digitalMarket);
                            processed.Add(currentMarket);
                            marketProcessed = true;
                            break;
                        }
                    }

                    // If no match, add as is (e.g., 'test')
                    if (!marketProcessed)
                    {
                        processedMarkets.Add(currentMarket);
                        processed.Add(currentMarket);
                    }
                }
            }

            return processedMarkets;
        }


        private string Method_UpdateMarketNameDigitalMarketMapping(IOrganizationService service, string marketname, Entity bd)
        {
            string[] marketArraySplitPlus = marketname.Split('+').Select(m => m.Trim()).ToArray();
            string marketfinal_name = "";
            HashSet<string> uniqueMarkets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Retrieve zx_id from campaign
            int zx_id = 0;
            if (bd.Contains("zx_campaign"))
            {
                Entity campaign = service.Retrieve("zx_campaign", ((EntityReference)bd["zx_campaign"]).Id, new ColumnSet("zx_campaigntype"));
                if (campaign.Contains("zx_campaigntype"))
                {
                    Entity campaignTypeRecord = service.Retrieve("zx_campaigntype", ((EntityReference)campaign["zx_campaigntype"]).Id, new ColumnSet("zx_id"));
                    zx_id = campaignTypeRecord.Contains("zx_id") ? Convert.ToInt32(campaignTypeRecord["zx_id"]) : 0;
                }
            }

            foreach (var market in marketArraySplitPlus)
            {
                if (string.IsNullOrWhiteSpace(market)) continue;

                string marketQuery = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
            <entity name='zx_markets'>
                <attribute name='zx_marketsid'/>
                <attribute name='zx_digitalmarketmapping'/>
                <attribute name='zx_digitalmarkets'/>
                <attribute name='zx_ruralmarket'/>
                <filter type='and'>
                    <condition attribute='statecode' operator='eq' value='0'/>
                    <filter type='or'>
                        <condition attribute='zx_digitalmarkets' operator='eq' value='{market}'/>
                        <condition attribute='zx_mashmarkets' operator='eq' value='{market}'/>
                        <condition attribute='zx_digitalmarketmapping' operator='eq' value='{market}'/>
                    </filter>
                </filter>
            </entity>
        </fetch>";

                EntityCollection marketData = service.RetrieveMultiple(new FetchExpression(marketQuery));

                string mappedMarket = market;
                if (marketData.Entities.Count > 0)
                {
                    Entity matchedMarket = marketData.Entities[0];
                    if (zx_id == 2 && matchedMarket.Contains("zx_ruralmarket") && !string.IsNullOrEmpty(matchedMarket["zx_ruralmarket"].ToString()))
                    {
                        mappedMarket = matchedMarket["zx_ruralmarket"].ToString();
                    }
                    else
                    {
                        string digitalMarketMapping = matchedMarket.Contains("zx_digitalmarketmapping") ? matchedMarket["zx_digitalmarketmapping"].ToString() : "";
                        string digitalMarkets = matchedMarket.Contains("zx_digitalmarkets") ? matchedMarket["zx_digitalmarkets"].ToString() : "";

                        if (!string.IsNullOrEmpty(digitalMarketMapping))
                        {
                            mappedMarket = digitalMarketMapping;
                        }
                        else if (!string.IsNullOrEmpty(digitalMarkets))
                        {
                            mappedMarket = digitalMarkets;
                        }
                    }
                }

                if (!uniqueMarkets.Contains(mappedMarket))
                {
                    uniqueMarkets.Add(mappedMarket);
                    marketfinal_name += (marketfinal_name == "" ? "" : "+") + mappedMarket;
                }
            }
            return marketfinal_name;
        }

        private int CalculateUniverseSize(IOrganizationService service, List<string> markets, Guid campaignTypeId)
        {
            int totalUniverseSize = 0;

            foreach (string market in markets)
            {
                if (string.IsNullOrEmpty(market)) continue;

                string marketQuery = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                    <entity name='zx_markets'>
                        <attribute name='zx_marketsid'/>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                            <filter type='or'>
                                <condition attribute='zx_digitalmarkets' operator='eq' value='{market}'/>
                                <condition attribute='zx_mashmarkets' operator='eq' value='{market}'/>
                                <condition attribute='zx_digitalmarketmapping' operator='eq' value='{market}'/>
                            </filter>
                        </filter>
                    </entity>
                </fetch>";

                EntityCollection marketData = service.RetrieveMultiple(new FetchExpression(marketQuery));
                if (marketData.Entities.Count == 0)
                {
                    continue;
                }

                Guid marketId = marketData.Entities[0].Id;

                string universeQuery = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                    <entity name='zx_yt_universe'>
                        <attribute name='zx_universe'/>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                            <condition attribute='zx_sumofallmarketsuniverse' operator='eq' value='1'/>
                            <condition attribute='zx_medium' operator='eq' value='{campaignTypeId}' uitype='zx_campaigntype'/>
                            <condition attribute='zx_market' operator='eq' value='{marketId}' uitype='zx_markets'/>
                        </filter>
                    </entity>
                </fetch>";

                EntityCollection universeData = service.RetrieveMultiple(new FetchExpression(universeQuery));
                if (universeData.Entities.Count > 0 && universeData.Entities[0].Contains("zx_universe"))
                {
                    totalUniverseSize += Convert.ToInt32(universeData.Entities[0]["zx_universe"]);
                }
            }
            return totalUniverseSize;
        }

        public Guid language(IOrganizationService service, string lang)
        {
            string languageenglish = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                <entity name='zx_creative_language'>
                    <attribute name='zx_creative_languageid'/>
                    <attribute name='zx_language'/>
                    <attribute name='createdon'/>
                    <filter type='and'>
                        <condition attribute='zx_language' operator='eq' value='{lang}'/>
                    </filter>
                </entity>
            </fetch>";
            EntityCollection languageenglishcol = service.RetrieveMultiple(new FetchExpression(languageenglish));
            Guid langId = languageenglishcol.Entities.Count > 0 ? languageenglishcol.Entities[0].Id : Guid.Empty;
            return langId;
        }

        //Regionallangaugeandcreative method

        public void regionallanguageandlanguagecreationmethod(IOrganizationService service,
     IPluginExecutionContext context,
     List<string> processedMarkets,
     string marketname_text,
     Entity bd,
     Entity campaign,
     int camp_selected,
     int zx_id)
        {
            if (processedMarkets == null || processedMarkets.Count == 0) return;
            if (context.Depth > 1) return;

            JArray finalLanguageArray = new JArray();
            HashSet<string> uniqueLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool isClusterAndCTV = bd.Contains("zx_markettype") && ((OptionSetValue)bd["zx_markettype"]).Value == 128780001 && zx_id == 0;
            bool isSingleMarket = bd.Contains("zx_markettype") && ((OptionSetValue)bd["zx_markettype"]).Value == 128780000;
            var restrictedLanguages = isClusterAndCTV ?
                new List<string> { "Odia", "Assamese", "Rajasthani" } :
                new List<string> { };

            HashSet<string> hindiUniversalMarkets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> englishUniversalMarkets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            string inputHindiUniversal = bd.Contains("zx_hindiuniversal") ? bd.GetAttributeValue<string>("zx_hindiuniversal")?.Trim() : null;
            string inputEnglishUniversal = bd.Contains("zx_englishuniversal") ? bd.GetAttributeValue<string>("zx_englishuniversal")?.Trim() : null;

            if (!isSingleMarket && (!string.IsNullOrEmpty(inputHindiUniversal) || !string.IsNullOrEmpty(inputEnglishUniversal)))
            {
                if (!string.IsNullOrEmpty(inputHindiUniversal))
                {
                    var markets = inputHindiUniversal.Split(new[] { '+', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(m => Method_UpdateMarketNameDigitalMarketMapping(service, m.Trim(), bd))
                        .Where(m => !string.IsNullOrEmpty(m));
                    hindiUniversalMarkets.UnionWith(markets);
                }
                if (!string.IsNullOrEmpty(inputEnglishUniversal))
                {
                    var markets = inputEnglishUniversal.Split(new[] { '+', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(m => Method_UpdateMarketNameDigitalMarketMapping(service, m.Trim(), bd))
                        .Where(m => !string.IsNullOrEmpty(m) && !hindiUniversalMarkets.Contains(m));
                    englishUniversalMarkets.UnionWith(markets);
                }
            }

            string existingGeoBudgetSplit = bd.Contains("zx_geobudgetsplitvalue") ? bd.GetAttributeValue<string>("zx_geobudgetsplitvalue")?.Trim() : "";
            bool hasGeoSplit = bd.Contains("zx_geotargetingsplitlookup") && bd["zx_geotargetingsplitlookup"] != null;
            bool isGeoBudgetSplitNotBlank = !string.IsNullOrEmpty(existingGeoBudgetSplit);

            if (isGeoBudgetSplitNotBlank)
            {
                uniqueLanguages.Clear();
                finalLanguageArray.Clear();

                Guid hindiLangId = language(service, "Hindi");
                Guid marathiLangId = language(service, "Marathi");
                if (hindiLangId != Guid.Empty)
                {
                    AddUniqueLanguage(finalLanguageArray, "Hindi", hindiLangId);
                    uniqueLanguages.Add("Hindi");
                }
                if (marathiLangId != Guid.Empty)
                {
                    AddUniqueLanguage(finalLanguageArray, "Marathi", marathiLangId);
                    uniqueLanguages.Add("Marathi");
                }

                bd["zx_regionallanguagetext"] = "Hindi;Marathi";
                bd["zx_regionallanguages"] = finalLanguageArray.ToString();
                bd["zx_geobudgetsplitvalue"] = existingGeoBudgetSplit;
                bd["zx_hindiuniversal"] = "";
                bd["zx_englishuniversal"] = "";

                if (hindiLangId != Guid.Empty)
                    CreateLanguageCreatives(service, campaign, hindiLangId);
                if (marathiLangId != Guid.Empty)
                    CreateLanguageCreatives(service, campaign, marathiLangId);
            }
            else if (!isSingleMarket)
            {
                string regionalLanguagesText = bd.Contains("zx_regionallanguagetext")
                    ? bd.GetAttributeValue<string>("zx_regionallanguagetext")?.Trim()
                    : string.Empty;

                if (!string.IsNullOrEmpty(regionalLanguagesText))
                {
                    List<string> inputRegionalLanguages = regionalLanguagesText
                        .Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(l => l.Trim())
                        .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    foreach (string lang in inputRegionalLanguages)
                    {
                        if (restrictedLanguages.Any(r => r.Equals(lang, StringComparison.OrdinalIgnoreCase)))
                            continue;

                        Guid langId = language(service, lang);
                        if (langId != Guid.Empty)
                        {
                            bool creativeAvailable = false;
                            if (campaign.Contains("zx_campaigntype") && campaign.Contains("zx_variant"))
                            {
                                string creativeFetch = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                            <entity name='zx_creative'>
                                <attribute name='zx_creativeid'/>
                                <filter type='and'>
                                    <condition attribute='statecode' operator='eq' value='0'/>
                                    <condition attribute='zx_medium' operator='eq' value='{((EntityReference)campaign["zx_campaigntype"]).Id}'/>
                                    <condition attribute='zx_variant' operator='eq' value='{((EntityReference)campaign["zx_variant"]).Id}'/>
                                    <condition attribute='zx_language' operator='eq' value='{langId}'/>
                                </filter>
                            </entity>
                        </fetch>";

                                EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                                creativeAvailable = creativeCollection.Entities.Count > 0;
                            }

                            if (creativeAvailable)
                            {
                                string standardizedLang = char.ToUpper(lang[0]) + lang.Substring(1).ToLower();
                                uniqueLanguages.Add(standardizedLang);
                                AddUniqueLanguage(finalLanguageArray, standardizedLang, langId);
                            }
                        }
                    }

                    bd["zx_regionallanguagetext"] = uniqueLanguages.Any() ? string.Join(";", uniqueLanguages) : "";
                    bd["zx_regionallanguages"] = finalLanguageArray.ToString();
                }
                else
                {
                    foreach (string marketname in processedMarkets)
                    {
                        string fetchXml = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                    <entity name='zx_stateregionallanguage'>
                        <attribute name='zx_regionallanguage'/>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                        </filter>
                        <link-entity name='zx_markets' alias='aa' link-type='inner' from='zx_marketsid' to='zx_state'>
                            <filter type='or'>
                                <condition attribute='zx_digitalmarkets' operator='eq' value='{marketname}'/>
                                <condition attribute='zx_mashmarkets' operator='eq' value='{marketname}'/>
                                <condition attribute='zx_digitalmarketmapping' operator='eq' value='{marketname}'/>
                            </filter>
                        </link-entity>
                    </entity>
                </fetch>";

                        EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                        List<string> availableLanguages = new List<string>();
                        foreach (Entity record in entityCollection.Entities)
                        {
                            if (record.Contains("zx_regionallanguage"))
                            {
                                EntityReference regionalLang = (EntityReference)record["zx_autoregionallanguage"];
                                string langName = regionalLang.Name;
                                availableLanguages.AddRange(langName.Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(l => l.Trim())
                                    .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower()));
                            }
                        }

                        availableLanguages = availableLanguages.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                        foreach (string lang in availableLanguages)
                        {
                            if (restrictedLanguages.Any(r => r.Equals(lang, StringComparison.OrdinalIgnoreCase)))
                                continue;

                            Guid langId = language(service, lang);
                            if (langId == Guid.Empty)
                                continue;

                            bool creativeAvailable = false;
                            if (campaign.Contains("zx_campaigntype") && campaign.Contains("zx_variant"))
                            {
                                string creativeFetch = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                            <entity name='zx_creative'>
                                <attribute name='zx_creativeid'/>
                                <filter type='and'>
                                    <condition attribute='statecode' operator='eq' value='0'/>
                                    <condition attribute='zx_medium' operator='eq' value='{((EntityReference)campaign["zx_campaigntype"]).Id}'/>
                                    <condition attribute='zx_variant' operator='eq' value='{((EntityReference)campaign["zx_variant"]).Id}'/>
                                    <condition attribute='zx_language' operator='eq' value='{langId}'/>
                                </filter>
                            </entity>
                        </fetch>";

                                EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                                creativeAvailable = creativeCollection.Entities.Count > 0;
                            }

                            if (creativeAvailable)
                            {
                                string standardizedLang = char.ToUpper(lang[0]) + lang.Substring(1).ToLower();
                                uniqueLanguages.Add(standardizedLang);
                                AddUniqueLanguage(finalLanguageArray, standardizedLang, langId);
                            }
                        }
                    }

                    bd["zx_regionallanguagetext"] = uniqueLanguages.Any() ? string.Join(";", uniqueLanguages) : "";
                    bd["zx_regionallanguages"] = finalLanguageArray.ToString();
                }

                bd["zx_commonlanguageforgeosplit"] = "";
                bd["zx_geobudgetsplitvalue"] = "";
            }
            else
            {
                string existingRegionalLanguages = bd.Contains("zx_regionallanguagetext") ? bd.GetAttributeValue<string>("zx_regionallanguagetext")?.Trim() : "";
                List<string> allRegionalLanguages = new List<string>();
                if (!string.IsNullOrEmpty(existingRegionalLanguages))
                {
                    allRegionalLanguages.AddRange(existingRegionalLanguages.Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
                }

                bool hindiCreativeAssigned = false;
                foreach (string marketname in processedMarkets)
                {
                    string fetchXml = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                <entity name='zx_stateregionallanguage'>
                    <attribute name='zx_regionallanguage'/>
                    <filter type='and'>
                        <condition attribute='statecode' operator='eq' value='0'/>
                    </filter>
                    <link-entity name='zx_markets' alias='aa' link-type='inner' from='zx_marketsid' to='zx_state'>
                        <filter type='or'>
                            <condition attribute='zx_digitalmarkets' operator='eq' value='{marketname}'/>
                            <condition attribute='zx_mashmarkets' operator='eq' value='{marketname}'/>
                            <condition attribute='zx_digitalmarketmapping' operator='eq' value='{marketname}'/>
                        </filter>
                    </link-entity>
                </entity>
            </fetch>";

                    EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                    List<string> availableLanguages = new List<string>();
                    foreach (Entity record in entityCollection.Entities)
                    {
                        if (record.Contains("zx_regionallanguage"))
                        {
                            EntityReference regionalLang = (EntityReference)record["zx_regionallanguage"];
                            string langName = regionalLang.Name;
                            availableLanguages.AddRange(langName.Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(l => l.Trim())
                                .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower()));
                        }
                    }

                    availableLanguages = availableLanguages.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                    bool hindiCreativeAvailable = false;
                    Guid hindiLangId = language(service, "Hindi");
                    if (availableLanguages.Any(l => l.Equals("Hindi", StringComparison.OrdinalIgnoreCase)) && hindiLangId != Guid.Empty)
                    {
                        if (campaign.Contains("zx_campaigntype") && campaign.Contains("zx_variant"))
                        {
                            string creativeFetch = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                        <entity name='zx_creative'>
                            <attribute name='zx_creativeid'/>
                            <filter type='and'>
                                <condition attribute='statecode' operator='eq' value='0'/>
                                <condition attribute='zx_medium' operator='eq' value='{((EntityReference)campaign["zx_campaigntype"]).Id}'/>
                                <condition attribute='zx_variant' operator='eq' value='{((EntityReference)campaign["zx_variant"]).Id}'/>
                                <condition attribute='zx_language' operator='eq' value='{hindiLangId}'/>
                            </filter>
                        </entity>
                    </fetch>";

                            EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                            hindiCreativeAvailable = creativeCollection.Entities.Count > 0;
                        }
                    }

                    if (hindiCreativeAvailable)
                    {
                        uniqueLanguages.Add("Hindi");
                        AddUniqueLanguage(finalLanguageArray, "Hindi", hindiLangId);
                        hindiCreativeAssigned = true;
                    }

                    foreach (string lang in availableLanguages)
                    {
                        if (lang.Equals("Hindi", StringComparison.OrdinalIgnoreCase) ||
                            lang.Equals("English", StringComparison.OrdinalIgnoreCase) ||
                            restrictedLanguages.Any(r => r.Equals(lang, StringComparison.OrdinalIgnoreCase)))
                            continue;

                        Guid langId = language(service, lang);
                        if (langId == Guid.Empty)
                            continue;

                        bool creativeAvailable = false;
                        if (campaign.Contains("zx_campaigntype") && campaign.Contains("zx_variant"))
                        {
                            string creativeFetch = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                        <entity name='zx_creative'>
                            <attribute name='zx_creativeid'/>
                            <filter type='and'>
                                <condition attribute='statecode' operator='eq' value='0'/>
                                <condition attribute='zx_medium' operator='eq' value='{((EntityReference)campaign["zx_campaigntype"]).Id}'/>
                                <condition attribute='zx_variant' operator='eq' value='{((EntityReference)campaign["zx_variant"]).Id}'/>
                                <condition attribute='zx_language' operator='eq' value='{langId}'/>
                            </filter>
                        </entity>
                    </fetch>";

                            EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                            creativeAvailable = creativeCollection.Entities.Count > 0;
                        }

                        if (creativeAvailable)
                        {
                            string standardizedLang = char.ToUpper(lang[0]) + lang.Substring(1).ToLower();
                            uniqueLanguages.Add(standardizedLang);
                            AddUniqueLanguage(finalLanguageArray, standardizedLang, langId);
                            break;
                        }
                    }
                }

                if (bd.Contains("zx_markettype"))
                {
                    int marketTypeValue = ((OptionSetValue)bd["zx_markettype"]).Value;

                    if (marketTypeValue == 128780000 && zx_id == 0)
                    {
                        string metroNonMetroQuery = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                    <entity name='zx_metrononmetrocitymaster'>
                        <attribute name='statecode'/>
                        <attribute name='zx_metrononmetrocitymasterid'/>
                        <attribute name='zx_name'/>
                        <attribute name='zx_market'/>
                        <attribute name='zx_newmetrocity'/>
                        <attribute name='zx_newnonmetrocity'/>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                        </filter>
                        <link-entity name='zx_markets' alias='aa' link-type='inner' from='zx_marketsid' to='zx_market'>
                            <filter type='and'>
                                <filter type='or'>
                                    <condition attribute='zx_digitalmarkets' operator='eq' value='{marketname_text}'/>
                                    <condition attribute='zx_digitalmarketmapping' operator='eq' value='{marketname_text}'/>
                                </filter>
                            </filter>
                        </link-entity>
                        <link-entity name='zx_marketabbreviationmaster' from='zx_marketabbreviationmasterid' to='zx_newmetrocity' link-type='outer' alias='alia_metromarketname'>
                            <attribute name='zx_marketname'/>
                        </link-entity>
                        <link-entity name='zx_marketabbreviationmaster' from='zx_marketabbreviationmasterid' to='zx_newnonmetrocity' link-type='outer' alias='alia_nonmetromarketname'>
                            <attribute name='zx_marketname'/>
                        </link-entity>
                    </entity>
                </fetch>";

                        EntityCollection metroNonMetroData = service.RetrieveMultiple(new FetchExpression(metroNonMetroQuery));

                        if (metroNonMetroData.Entities.Count > 0 && hasGeoSplit)
                        {
                            string commonLanguage = bd.GetAttributeValue<string>("zx_commonlanguageforgeosplit")?.Trim();
                            if (!string.IsNullOrEmpty(commonLanguage))
                            {
                                string standardizedCommonLang = char.ToUpper(commonLanguage[0]) + commonLanguage.Substring(1).ToLower();
                                Guid langId = language(service, standardizedCommonLang);
                                if (langId != Guid.Empty && !uniqueLanguages.Contains(standardizedCommonLang))
                                {
                                    AddUniqueLanguage(finalLanguageArray, standardizedCommonLang, langId);
                                    uniqueLanguages.Add(standardizedCommonLang);
                                }
                            }
                            if (allRegionalLanguages.Any())
                            {
                                foreach (var lang in allRegionalLanguages)
                                {
                                    if (lang.Equals("English", StringComparison.OrdinalIgnoreCase) && uniqueLanguages.Contains("Hindi"))
                                        continue;
                                    Guid langId = language(service, lang);
                                    if (langId != Guid.Empty && !uniqueLanguages.Contains(lang))
                                    {
                                        AddUniqueLanguage(finalLanguageArray, lang, langId);
                                        uniqueLanguages.Add(lang);
                                    }
                                }
                            }
                            bd["zx_regionallanguages"] = finalLanguageArray.ToString();
                            bd["zx_regionallanguagetext"] = string.Join(";", uniqueLanguages);
                            bd["zx_commonlanguageforgeosplit"] = commonLanguage ?? "";
                        }
                        else
                        {
                            if (allRegionalLanguages.Any())
                            {
                                foreach (var lang in allRegionalLanguages)
                                {
                                    if (lang.Equals("English", StringComparison.OrdinalIgnoreCase) && uniqueLanguages.Contains("Hindi"))
                                        continue;
                                    Guid langId = language(service, lang);
                                    if (langId != Guid.Empty && !uniqueLanguages.Contains(lang))
                                    {
                                        AddUniqueLanguage(finalLanguageArray, lang, langId);
                                        uniqueLanguages.Add(lang);
                                    }
                                }
                            }
                            bd["zx_regionallanguages"] = finalLanguageArray.ToString();
                            bd["zx_regionallanguagetext"] = string.Join(";", uniqueLanguages);
                            bd["zx_commonlanguageforgeosplit"] = "";
                            bd["zx_geobudgetsplitvalue"] = "";
                        }
                    }
                }

                bd["zx_hindiuniversal"] = "";
                bd["zx_englishuniversal"] = "";
            }

            if (!isSingleMarket && hindiUniversalMarkets.Count == 0 && englishUniversalMarkets.Count == 0)
            {
                ProcessHindiEnglishRegionalLanguages(service, bd, campaign, processedMarkets, finalLanguageArray, uniqueLanguages, hindiUniversalMarkets, englishUniversalMarkets, restrictedLanguages, zx_id);
            }

            bd["zx_hindiuniversal"] = hindiUniversalMarkets.Any() ? string.Join(";", hindiUniversalMarkets) : "";
            bd["zx_englishuniversal"] = englishUniversalMarkets.Any() ? string.Join(";", englishUniversalMarkets) : "";

            bd["zx_regionallanguages"] = finalLanguageArray.Any() ? finalLanguageArray.ToString() : "";

            if (!string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_regionallanguagetext")) ||
                !string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_hindiuniversal")) ||
                !string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_englishuniversal")))
            {
                bd["zx_commonlanguageforgeosplit"] = "";
            }

            if (isSingleMarket && bd.Contains("zx_regionallanguagetext") &&
                bd.GetAttributeValue<string>("zx_regionallanguagetext")?.Trim().Equals("English", StringComparison.OrdinalIgnoreCase) == true)
            {
                PatchRegionalLanguageForSingleMarketWithEnglish(service, bd, campaign, processedMarkets, finalLanguageArray, uniqueLanguages);
                bd["zx_regionallanguages"] = finalLanguageArray.Any() ? finalLanguageArray.ToString() : "";
                bd["zx_regionallanguagetext"] = uniqueLanguages.Any() ? string.Join(";", uniqueLanguages) : "";
            }

            bd["zx_name"] = Method_UpdateMarketNameDigitalMarketMapping(service, marketname_text, bd);

            if (bd.Contains("zx_markettype"))
            {
                int marketTypeValue = ((OptionSetValue)bd["zx_markettype"]).Value;
                bd["zx_newmarkettypelookup"] = Method_UpdateMarkettypeLookUp(service, (OptionSetValue)bd["zx_markettype"], bd, zx_id);

                if (!isSingleMarket && bd.Contains("zx_hindiuniversal") && bd.GetAttributeValue<string>("zx_hindiuniversal") != null)
                {
                    bd["zx_hindiuniversal"] = Method_UpdateMarketNameDigitalMarketMapping(service, bd.GetAttributeValue<string>("zx_hindiuniversal").Replace(";", "+"), bd);
                }
                if (!isSingleMarket && bd.Contains("zx_englishuniversal") && bd.GetAttributeValue<string>("zx_englishuniversal") != null)
                {
                    bd["zx_englishuniversal"] = Method_UpdateMarketNameDigitalMarketMapping(service, bd.GetAttributeValue<string>("zx_englishuniversal").Replace(";", "+"), bd);
                }
            }

            try
            {
                service.Update(bd);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Failed to update zx_geodetails: {ex.Message}", ex);
            }

            // Create creatives only for languages in uniqueLanguages or explicitly in universal fields
            foreach (string lang in uniqueLanguages)
            {
                Guid langId = language(service, lang);
                if (langId != Guid.Empty)
                    CreateLanguageCreatives(service, campaign, langId);
            }

            // For cluster market types, create languages and creatives for non-empty universal and common language fields
            if (!isSingleMarket && bd.Contains("zx_markettype") && ((OptionSetValue)bd["zx_markettype"]).Value == 128780001)
            {
                // Handle zx_hindiuniversal
                if (!string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_hindiuniversal")))
                {
                    Guid hindiLangId = language(service, "Hindi");
                    if (hindiLangId != Guid.Empty)
                    {
                        CreateLanguageCreatives(service, campaign, hindiLangId);
                    }
                }

                // Handle zx_englishuniversal
                if (!string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_englishuniversal")))
                {
                    Guid englishLangId = language(service, "English");
                    if (englishLangId != Guid.Empty)
                    {
                        CreateLanguageCreatives(service, campaign, englishLangId);
                    }
                }

                // Handle zx_commonlanguageforgeosplit
                string commonLanguage = bd.GetAttributeValue<string>("zx_commonlanguageforgeosplit")?.Trim();
                if (!string.IsNullOrEmpty(commonLanguage))
                {
                    string standardizedCommonLang = char.ToUpper(commonLanguage[0]) + commonLanguage.Substring(1).ToLower();
                    Guid commonLangId = language(service, standardizedCommonLang);
                    if (commonLangId != Guid.Empty)
                    {
                        CreateLanguageCreatives(service, campaign, commonLangId);
                    }
                }
            }

            MethodCampaignWithIssueStatusUpdate(service, bd, campaign, processedMarkets);
        }





        //Single Regional English Process
        private void PatchRegionalLanguageForSingleMarketWithEnglish(IOrganizationService service, Entity bd, Entity campaign, List<string> processedMarkets, JArray finalLanguageArray, HashSet<string> uniqueLanguages)
        {
            finalLanguageArray.Clear();
            uniqueLanguages.Clear();

            Guid campaignTypeId = Guid.Empty;
            Guid variantId = Guid.Empty;

            // Retrieve campaign type and variant IDs safely
            if (campaign.Contains("zx_campaigntype") && campaign["zx_campaigntype"] != null)
            {
                campaignTypeId = ((EntityReference)campaign["zx_campaigntype"]).Id;
            }
            if (campaign.Contains("zx_variant") && campaign["zx_variant"] != null)
            {
                variantId = ((EntityReference)campaign["zx_variant"]).Id;
            }

            // Step 1: Handle missing campaign details
            if (campaignTypeId == Guid.Empty || variantId == Guid.Empty)
            {
                Guid englishLangId_1 = language(service, "English");
                if (englishLangId_1 != Guid.Empty)
                {
                    uniqueLanguages.Add("English");
                    AddUniqueLanguage(finalLanguageArray, "English", englishLangId_1);
                    CreateLanguageCreatives(service, campaign, englishLangId_1);
                }
                else
                {
                    // Log or handle case where English language is not found
                    uniqueLanguages.Add("English");
                    AddUniqueLanguage(finalLanguageArray, "English", Guid.Empty); // Use empty GUID as fallback
                }
                return;
            }

            bool regionalCreativeFound = false;
            string selectedRegionalLanguage = null;
            Guid selectedRegionalLangId = Guid.Empty;

            // Step 2: Check regional languages
            foreach (string marketname in processedMarkets)
            {
                string fetchXml = "<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>" +
                                  "<entity name='zx_stateregionallanguage'>" +
                                  "<attribute name='zx_regionallanguage'/>" +
                                  "<filter type='and'>" +
                                  "<condition attribute='statecode' operator='eq' value='0'/>" +
                                  "</filter>" +
                                  "<link-entity name='zx_markets' alias='aa' link-type='inner' from='zx_marketsid' to='zx_state'>" +
                                  "<filter type='or'>" +
                                  "<condition attribute='zx_digitalmarkets' operator='eq' value='" + marketname + "'/>" +
                                  "<condition attribute='zx_mashmarkets' operator='eq' value='" + marketname + "'/>" +
                                  "<condition attribute='zx_digitalmarketmapping' operator='eq' value='" + marketname + "'/>" +
                                  "</filter>" +
                                  "</link-entity>" +
                                  "</entity>" +
                                  "</fetch>";

                EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                List<string> availableLanguages = new List<string>();

                foreach (Entity record in entityCollection.Entities)
                {
                    if (record.Contains("zx_regionallanguage"))
                    {
                        EntityReference regionalLang = (EntityReference)record["zx_regionallanguage"];
                        string langName = regionalLang.Name;
                        availableLanguages.AddRange(langName.Split(new char[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(l => l.Trim())
                            .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower()));
                    }
                }

                availableLanguages = availableLanguages.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                foreach (string lang in availableLanguages)
                {
                    if (lang.Equals("English", StringComparison.OrdinalIgnoreCase))
                        continue; // Skip English for regional check

                    Guid langId = language(service, lang);
                    if (langId == Guid.Empty)
                        continue;

                    string creativeFetch = "<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>" +
                                          "<entity name='zx_creative'>" +
                                          "<attribute name='zx_creativeid'/>" +
                                          "<filter type='and'>" +
                                          "<condition attribute='statecode' operator='eq' value='0'/>" +
                                          "<condition attribute='zx_medium' operator='eq' value='" + campaignTypeId.ToString() + "'/>" +
                                          "<condition attribute='zx_variant' operator='eq' value='" + variantId.ToString() + "'/>" +
                                          "<condition attribute='zx_language' operator='eq' value='" + langId.ToString() + "'/>" +
                                          "</filter>" +
                                          "</entity>" +
                                          "</fetch>";

                    EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                    if (creativeCollection.Entities.Count > 0)
                    {
                        regionalCreativeFound = true;
                        selectedRegionalLanguage = char.ToUpper(lang[0]) + lang.Substring(1).ToLower();
                        selectedRegionalLangId = langId;
                        break;
                    }
                }

                if (regionalCreativeFound)
                    break;
            }

            if (regionalCreativeFound && !string.IsNullOrEmpty(selectedRegionalLanguage) && selectedRegionalLangId != Guid.Empty)
            {
                uniqueLanguages.Add(selectedRegionalLanguage);
                AddUniqueLanguage(finalLanguageArray, selectedRegionalLanguage, selectedRegionalLangId);
                CreateLanguageCreatives(service, campaign, selectedRegionalLangId);
                return;
            }

            // Step 3: Check universal languages
            bool universalCreativeFound = false;
            string universalLanguage = null;
            Guid universalLangId = Guid.Empty;

            foreach (string marketname in processedMarkets)
            {
                string fetchXml = "<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>" +
                                  "<entity name='zx_stateregionallanguage'>" +
                                  "<attribute name='zx_universallanguage'/>" +
                                  "<filter type='and'>" +
                                  "<condition attribute='statecode' operator='eq' value='0'/>" +
                                  "</filter>" +
                                  "<link-entity name='zx_markets' alias='aa' link-type='inner' from='zx_marketsid' to='zx_state'>" +
                                  "<filter type='or'>" +
                                  "<condition attribute='zx_digitalmarkets' operator='eq' value='" + marketname + "'/>" +
                                  "<condition attribute='zx_mashmarkets' operator='eq' value='" + marketname + "'/>" +
                                  "<condition attribute='zx_digitalmarketmapping' operator='eq' value='" + marketname + "'/>" +
                                  "</filter>" +
                                  "</link-entity>" +
                                  "</entity>" +
                                  "</fetch>";

                EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                List<string> availableLanguages = new List<string>();

                foreach (Entity record in entityCollection.Entities)
                {
                    if (record.Contains("zx_universallanguage"))
                    {
                        EntityReference universalLang = (EntityReference)record["zx_universallanguage"];
                        string langName = universalLang.Name;
                        if (!string.IsNullOrEmpty(langName))
                        {
                            availableLanguages.AddRange(langName.Split(new char[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(l => l.Trim())
                                .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower()));
                        }
                    }
                }

                availableLanguages = availableLanguages.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                // Prioritize Hindi, then other languages
                foreach (string lang in availableLanguages.OrderBy(l => l.Equals("Hindi", StringComparison.OrdinalIgnoreCase) ? 0 : 1))
                {
                    if (lang.Equals("English", StringComparison.OrdinalIgnoreCase))
                        continue; // Skip English for universal check

                    universalLangId = language(service, lang);
                    if (universalLangId == Guid.Empty)
                        continue;

                    string creativeFetch = "<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>" +
                                          "<entity name='zx_creative'>" +
                                          "<attribute name='zx_creativeid'/>" +
                                          "<filter type='and'>" +
                                          "<condition attribute='statecode' operator='eq' value='0'/>" +
                                          "<condition attribute='zx_medium' operator='eq' value='" + campaignTypeId.ToString() + "'/>" +
                                          "<condition attribute='zx_variant' operator='eq' value='" + variantId.ToString() + "'/>" +
                                          "<condition attribute='zx_language' operator='eq' value='" + universalLangId.ToString() + "'/>" +
                                          "</filter>" +
                                          "</entity>" +
                                          "</fetch>";

                    EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                    if (creativeCollection.Entities.Count > 0)
                    {
                        universalCreativeFound = true;
                        universalLanguage = char.ToUpper(lang[0]) + lang.Substring(1).ToLower();
                        break;
                    }
                }

                if (universalCreativeFound)
                    break;
            }

            if (universalCreativeFound && !string.IsNullOrEmpty(universalLanguage) && universalLangId != Guid.Empty)
            {
                uniqueLanguages.Add(universalLanguage);
                AddUniqueLanguage(finalLanguageArray, universalLanguage, universalLangId);
                CreateLanguageCreatives(service, campaign, universalLangId);
                return;
            }

            // Step 4: Default to English
            Guid englishLangId = language(service, "English");
            if (englishLangId != Guid.Empty)
            {
                uniqueLanguages.Add("English");
                AddUniqueLanguage(finalLanguageArray, "English", englishLangId);
                CreateLanguageCreatives(service, campaign, englishLangId);
            }
            else
            {
                // Fallback if English language is not found
                uniqueLanguages.Add("English");
                AddUniqueLanguage(finalLanguageArray, "English", Guid.Empty);
            }
        }




        //GEOSPLITVALUE UNIVERSAL FIELD UPDATES FOR HINDI AND ENGLISH
        private void ProcessHindiEnglishRegionalLanguages(IOrganizationService service, Entity bd, Entity campaign, List<string> processedMarkets, JArray finalLanguageArray, HashSet<string> uniqueLanguages, HashSet<string> hindiUniversalMarkets, HashSet<string> englishUniversalMarkets, List<string> restrictedLanguages, int zx_id)
        {
            bool isGeoBudgetSplitNotBlank = bd.Contains("zx_geobudgetsplitvalue") && !string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_geobudgetsplitvalue")?.Trim());
            if (isGeoBudgetSplitNotBlank)
            {
                return;
            }

            bool isSingleMarket = bd.Contains("zx_markettype") && ((OptionSetValue)bd["zx_markettype"]).Value == 128780000;

            if (isSingleMarket)
            {
                hindiUniversalMarkets.Clear();
                englishUniversalMarkets.Clear();
                bd["zx_hindiuniversal"] = "";
                bd["zx_englishuniversal"] = "";
                return;
            }

            hindiUniversalMarkets.Clear();
            englishUniversalMarkets.Clear();

            foreach (string marketname in processedMarkets)
            {
                string mappedMarket = Method_UpdateMarketNameDigitalMarketMapping(service, marketname, bd);
                string fetchXml = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
            <entity name='zx_stateregionallanguage'>
                <attribute name='zx_regionallanguage'/>
                <filter type='and'>
                    <condition attribute='statecode' operator='eq' value='0'/>
                </filter>
                <link-entity name='zx_markets' alias='aa' link-type='inner' from='zx_marketsid' to='zx_state'>
                    <filter type='or'>
                        <condition attribute='zx_digitalmarkets' operator='eq' value='{marketname}'/>
                        <condition attribute='zx_mashmarkets' operator='eq' value='{marketname}'/>
                        <condition attribute='zx_digitalmarketmapping' operator='eq' value='{marketname}'/>
                    </filter>
                </link-entity>
            </entity>
        </fetch>";

                EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                List<string> availableLanguages = new List<string>();
                foreach (Entity record in entityCollection.Entities)
                {
                    if (record.Contains("zx_regionallanguage"))
                    {
                        EntityReference regionalLang = (EntityReference)record["zx_regionallanguage"];
                        string langName = regionalLang.Name;
                        availableLanguages.AddRange(langName.Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(l => l.Trim())
                            .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower()));
                    }
                }

                availableLanguages = availableLanguages.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                bool hindiCreativeAvailable = false;
                Guid hindiLangId = language(service, "Hindi");
                if (availableLanguages.Any(l => l.Equals("Hindi", StringComparison.OrdinalIgnoreCase)) && hindiLangId != Guid.Empty)
                {
                    if (campaign.Contains("zx_campaigntype") && campaign.Contains("zx_variant"))
                    {
                        string creativeFetch = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                    <entity name='zx_creative'>
                        <attribute name='zx_creativeid'/>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                            <condition attribute='zx_medium' operator='eq' value='{((EntityReference)campaign["zx_campaigntype"]).Id}'/>
                            <condition attribute='zx_variant' operator='eq' value='{((EntityReference)campaign["zx_variant"]).Id}'/>
                            <condition attribute='zx_language' operator='eq' value='{hindiLangId}'/>
                        </filter>
                    </entity>
                </fetch>";

                        EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                        hindiCreativeAvailable = creativeCollection.Entities.Count > 0;
                    }
                }

                if (hindiCreativeAvailable)
                {
                    hindiUniversalMarkets.Add(mappedMarket);
                    continue;
                }

                bool englishCreativeAvailable = false;
                Guid englishLangId = language(service, "English");
                if (availableLanguages.Any(l => l.Equals("English", StringComparison.OrdinalIgnoreCase)) && englishLangId != Guid.Empty)
                {
                    if (campaign.Contains("zx_campaigntype") && campaign.Contains("zx_variant"))
                    {
                        string creativeFetch = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                    <entity name='zx_creative'>
                        <attribute name='zx_creativeid'/>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                            <condition attribute='zx_medium' operator='eq' value='{((EntityReference)campaign["zx_campaigntype"]).Id}'/>
                            <condition attribute='zx_variant' operator='eq' value='{((EntityReference)campaign["zx_variant"]).Id}'/>
                            <condition attribute='zx_language' operator='eq' value='{englishLangId}'/>
                        </filter>
                    </entity>
                </fetch>";

                        EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));
                        englishCreativeAvailable = creativeCollection.Entities.Count > 0;
                    }
                }

                if (englishCreativeAvailable)
                {
                    englishUniversalMarkets.Add(mappedMarket);
                }
            }
        }




        //Method for Campaign Status 
        private void MethodCampaignWithIssueStatusUpdate(IOrganizationService service, Entity bd, Entity campaign_1, List<string> processMarkets)
        {
            if (!bd.Contains("zx_campaign") || !(bd["zx_campaign"] is EntityReference campaignRef))
                return;

            // Retrieve campaign
            Entity campaign = service.Retrieve("zx_campaign", campaignRef.Id, new ColumnSet("zx_remark"));
            string existingRemarks = campaign.Contains("zx_remark") ? campaign.GetAttributeValue<string>("zx_remark") : string.Empty;

            // Fetch all zx_geodetails records for the campaign
            string fetchXml = $@"
        <fetch version='1.0' mapping='logical' distinct='false'>
            <entity name='zx_geodetails'>
                <attribute name='zx_name'/>
                <attribute name='zx_targetcpm'/>
                <attribute name='zx_markettype'/>
                <attribute name='zx_hindiuniversal'/>
                <attribute name='zx_englishuniversal'/>
                <filter type='and'>
                    <condition attribute='zx_campaign' operator='eq' value='{campaignRef.Id}'/>
                    <condition attribute='statecode' operator='eq' value='0'/>
                </filter>
            </entity>
        </fetch>";

            EntityCollection geoDetails = service.RetrieveMultiple(new FetchExpression(fetchXml));
            List<string> remarkParts = new List<string>();
            HashSet<string> allMarketsNotFound = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> allZeroCpmMarkets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Process each zx_geodetails record
            foreach (Entity geo in geoDetails.Entities)
            {
                if (geo.Contains("zx_markettype") && geo.GetAttributeValue<OptionSetValue>("zx_markettype")?.Value == 128780001)
                {
                    // Get the raw market name text and map it
                    string marketname_text = geo.Contains("zx_name") ? geo.GetAttributeValue<string>("zx_name") : string.Empty;
                    var marketNames = string.IsNullOrEmpty(marketname_text)
                        ? new List<string>()
                        : Method_UpdateMarketNameDigitalMarketMapping(service, marketname_text, geo)
                            .Split('+').Select(m => m.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                    // Get universal fields
                    string hindiUniversalFields_old_names = geo.Contains("zx_hindiuniversal") ? geo.GetAttributeValue<string>("zx_hindiuniversal") : string.Empty;
                    var hindiUniversalFields = string.IsNullOrEmpty(hindiUniversalFields_old_names)
                        ? new List<string>()
                        : Method_UpdateMarketNameDigitalMarketMapping(service, hindiUniversalFields_old_names, geo)
                            .Split('+').Select(m => m.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                    string englishUniversalFields_old_names = geo.Contains("zx_englishuniversal") ? geo.GetAttributeValue<string>("zx_englishuniversal") : string.Empty;
                    var englishUniversalFields = string.IsNullOrEmpty(englishUniversalFields_old_names)
                        ? new List<string>()
                        : Method_UpdateMarketNameDigitalMarketMapping(service, englishUniversalFields_old_names, geo)
                            .Split('+').Select(m => m.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                    // Find markets not covered by universal languages
                    var marketsNotFound = marketNames
                        .Where(m => !hindiUniversalFields.Contains(m, StringComparer.OrdinalIgnoreCase) &&
                                    !englishUniversalFields.Contains(m, StringComparer.OrdinalIgnoreCase))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    // Only add to allMarketsNotFound if there are uncovered markets
                    if (marketsNotFound.Any())
                    {
                        allMarketsNotFound.UnionWith(marketsNotFound);
                    }
                }

                // Check for zero CPM
                if (geo.Contains("zx_targetcpm") && geo.GetAttributeValue<decimal>("zx_targetcpm") == 0)
                {
                    var cpmMarketNames = geo.Contains("zx_name")
                        ? geo.GetAttributeValue<string>("zx_name")?.Split('+').Select(m => m.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>()
                        : new List<string>();
                    allZeroCpmMarkets.UnionWith(cpmMarketNames);
                }
            }

            // Add remarks only if there are issues
            if (allMarketsNotFound.Any())
            {
                remarkParts.Add($"Universal Language Creative Not Found in the Markets({string.Join(",", allMarketsNotFound)})");
            }
            if (allZeroCpmMarkets.Any())
            {
                remarkParts.Add($"CPM is Zero Markets({string.Join(",", allZeroCpmMarkets)})");
            }

            // Update campaign only if there are remarks to add
            if (remarkParts.Any())
            {
                string newRemarks = string.Join(" : ", remarkParts);
                string updatedRemarks = newRemarks; // Overwrite existing remarks

                Entity campaignToUpdate = new Entity("zx_campaign", campaignRef.Id)
                {
                    ["zx_remark"] = updatedRemarks,
                    ["zx_campaignstatus"] = new OptionSetValue(128780001) // Issue status
                };
                service.Update(campaignToUpdate);
            }
            else
            {
                // Clear remarks and reset status if no issues are found
                if (!string.IsNullOrEmpty(existingRemarks))
                {
                    Entity campaignToUpdate = new Entity("zx_campaign", campaignRef.Id)
                    {
                        ["zx_remark"] = null,
                        ["zx_campaignstatus"] = new OptionSetValue(128780000) // Default or active status
                    };
                    service.Update(campaignToUpdate);
                }
            }
        }

        // New method for non-Cluster + CTV campaigns
        private void ProcessNonClusterCTVLanguages(IOrganizationService service, IPluginExecutionContext context, Entity bd, Entity campaign)
        {
            JArray finalLanguageArray = new JArray();
            HashSet<string> uniqueLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            // Check if zx_geotargetingsplitlookup is not null and zx_commonlanguageforgeosplit has data
            bool hasGeoSplit = bd.Contains("zx_geotargetingsplitlookup") && bd["zx_geotargetingsplitlookup"] != null;
            string commonLanguage = bd.Contains("zx_commonlanguageforgeosplit")
                ? bd.GetAttributeValue<string>("zx_commonlanguageforgeosplit")?.Trim()
                : string.Empty;
            bool hasCommonLanguage = !string.IsNullOrEmpty(commonLanguage);

            if (hasGeoSplit && hasCommonLanguage)
            {
                // Preserve zx_commonlanguageforgeosplit and keep regional fields empty

                bd["zx_regionallanguages"] = "";
                bd["zx_regionallanguagetext"] = "";
                // Add common language to uniqueLanguages for creative creation
                string standardizedCommonLang = char.ToUpper(commonLanguage[0]) + commonLanguage.Substring(1).ToLower();
                uniqueLanguages.Add(standardizedCommonLang);
            }
            else
            {
                // Process regional languages from zx_regionallanguagetext
                string regionalLanguagesText = bd.Contains("zx_regionallanguagetext")
                    ? bd.GetAttributeValue<string>("zx_regionallanguagetext")?.Trim()
                    : string.Empty;

                if (!string.IsNullOrEmpty(regionalLanguagesText))
                {
                    // Split by semicolon or plus, standardize case
                    List<string> regionalLanguages = regionalLanguagesText
                        .Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(l => l.Trim())
                        .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    foreach (string lang in regionalLanguages)
                    {
                        Guid langId = language(service, lang);
                        if (langId != Guid.Empty)
                        {
                            AddUniqueLanguage(finalLanguageArray, lang, langId);
                            uniqueLanguages.Add(lang);
                        }
                    }
                }

                // Process common language from zx_commonlanguageforgeosplit (if not already handled)
                if (!hasGeoSplit && hasCommonLanguage)
                {
                    string standardizedCommonLang = char.ToUpper(commonLanguage[0]) + commonLanguage.Substring(1).ToLower();
                    if (!uniqueLanguages.Contains(standardizedCommonLang))
                    {
                        Guid langId = language(service, standardizedCommonLang);
                        if (langId != Guid.Empty)
                        {
                            AddUniqueLanguage(finalLanguageArray, standardizedCommonLang, langId);
                            uniqueLanguages.Add(standardizedCommonLang);
                        }
                    }
                }

                // Handle single market type with default language
                if (bd.Contains("zx_markettype") && ((OptionSetValue)bd["zx_markettype"]).Value == 128780000 &&
                    !hasGeoSplit &&
                    (string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_regionallanguages")) || !bd.Contains("zx_regionallanguages")) &&
                    string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_regionallanguagetext")) &&
                    !hasCommonLanguage)
                {
                    string defaultLanguage = "English";
                    Guid langId = language(service, defaultLanguage);
                    if (langId != Guid.Empty)
                    {
                        finalLanguageArray.Clear();
                        uniqueLanguages.Clear();
                        AddUniqueLanguage(finalLanguageArray, defaultLanguage, langId);
                        uniqueLanguages.Add(defaultLanguage);
                        bd["zx_regionallanguages"] = finalLanguageArray.ToString();
                        bd["zx_regionallanguagetext"] = string.Join(";", uniqueLanguages);
                        bd["zx_commonlanguageforgeosplit"] = "";
                    }
                }

                // Update fields based on collected languages
                // bd["zx_regionallanguages"] = finalLanguageArray.ToString();
                //bd["zx_regionallanguagetext"] = string.Join(";", uniqueLanguages);
            }

            // Create language creatives if on create
            if (context.MessageName.ToLower() == "create")
            {
                foreach (string lang in uniqueLanguages)
                {
                    Guid langId = language(service, lang);
                    if (langId != Guid.Empty)
                    {
                        CreateLanguageCreatives(service, campaign, langId);
                    }
                }
            }
            bool isSingleMarket = bd.Contains("zx_markettype") && ((OptionSetValue)bd["zx_markettype"]).Value == 128780000;
            if (isSingleMarket)
            {
                bd["zx_hindiuniversal"] = "";
                bd["zx_englishuniversal"] = "";
            }

        }


        private void Method_Regional_New(IOrganizationService service, IPluginExecutionContext context, OptionSetValue markettype, Entity bd, int zx_id, List<string> processedMarkets, Entity campaign)
        {
            // Initialize collections for languages
            JArray finalLanguageArray = new JArray();
            HashSet<string> uniqueLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool isClusterAndCTV = markettype != null && markettype.Value == 128780001 && zx_id == 0;
            var restrictedLanguages = isClusterAndCTV ?
                new List<string> { "Odia", "Assamese", "Rajasthani", "Hindi", "English" } :
                new List<string> { "Odia", "Assamese", "Rajasthani", "Hindi", "English" };

            // Initialize universal fields
            HashSet<string> hindiUniversalMarkets = bd.Contains("zx_hindiuniversal") && !string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_hindiuniversal"))
                ? new HashSet<string>(bd.GetAttributeValue<string>("zx_hindiuniversal").Split('+').Select(m => m.Trim()), StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> englishUniversalMarkets = bd.Contains("zx_englishuniversal") && !string.IsNullOrEmpty(bd.GetAttributeValue<string>("zx_englishuniversal"))
                ? new HashSet<string>(bd.GetAttributeValue<string>("zx_englishuniversal").Split('+').Select(m => m.Trim()), StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Step 1: Process regional languages from markets
            /*
            foreach (string marketname in processedMarkets)
            {
                string fetchXml = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                    <entity name='zx_stateregionallanguage'>
                        <attribute name='zx_name'/>
                        <attribute name='statecode'/>
                        <attribute name='zx_stateregionallanguageid'/>
                        <attribute name='zx_country'/>
                        <attribute name='zx_state'/>
                        <attribute name='zx_regionallanguage'/>
                        <attribute name='zx_universallanguage'/>
                        <filter type='and'>
                            <condition attribute='statecode' operator='eq' value='0'/>
                        </filter>
                        <link-entity name='zx_markets' alias='aa' link-type='inner' from='zx_marketsid' to='zx_state'>
                            <filter type='or'>
                                <condition attribute='zx_digitalmarkets' operator='eq' value='{marketname}'/>
                                <condition attribute='zx_mashmarkets' operator='eq' value='{marketname}'/>
                                <condition attribute='zx_digitalmarketmapping' operator='eq' value='{marketname}'/>
                            </filter>
                        </link-entity>
                    </entity>
                </fetch>";

                EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));

                foreach (Entity record in entityCollection.Entities)
                {
                    if (record.Contains("zx_regionallanguage"))
                    {
                        EntityReference regionalLang = (EntityReference)record["zx_regionallanguage"];
                        string langName = regionalLang.Name;

                        // Split combined languages (e.g., "Hindi;English")
                        string[] languages = langName.Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(l => l.Trim())
                            .ToArray();

                        // Get the mapped market name
                        string mappedMarket = Method_UpdateMarketNameDigitalMarketMapping(service, marketname, bd);

                        foreach (string lang in languages)
                        {
                            // For Cluster + CTV: Exclude restricted languages from regional fields
                            if (restrictedLanguages.Contains(lang, StringComparer.OrdinalIgnoreCase))
                            {
                                // Handle Hindi and English for universal fields
                                if (lang.Equals("Hindi", StringComparison.OrdinalIgnoreCase))
                                {
                                    hindiUniversalMarkets.Add(mappedMarket);
                                }
                                if (lang.Equals("English", StringComparison.OrdinalIgnoreCase))
                                {
                                    englishUniversalMarkets.Add(mappedMarket);
                                }
                                continue;
                            }

                            // Add to unique languages if not already present
                            if (!uniqueLanguages.Contains(lang))
                            {
                                Guid langId = language(service, lang);
                                if (langId != Guid.Empty)
                                {
                                    AddUniqueLanguage(finalLanguageArray, lang, langId);
                                    uniqueLanguages.Add(lang);
                                }
                            }
                        }
                    }
                }
            }
            */

            // Update universal fields
            bd["zx_hindiuniversal"] = hindiUniversalMarkets.Any() ? string.Join("+", hindiUniversalMarkets) : "";
            bd["zx_englishuniversal"] = englishUniversalMarkets.Any() ? string.Join("+", englishUniversalMarkets) : "";

            // Step 2: Process zx_regionallanguagetext field
            string regionalLanguagesText = bd.Contains("zx_regionallanguagetext")
                ? bd.GetAttributeValue<string>("zx_regionallanguagetext")?.Trim()
                : string.Empty;

            string marketName = bd.Contains("zx_name") ? bd.GetAttributeValue<string>("zx_name") : string.Empty;

            if (!string.IsNullOrEmpty(regionalLanguagesText))
            {
                // Split by semicolon or plus, standardize case
                List<string> regionalLanguages = regionalLanguagesText
                    .Split(new[] { ';', '+' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Trim())
                    .Select(l => char.ToUpper(l[0]) + l.Substring(1).ToLower())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // Remove Hindi and English from regional languages
                regionalLanguages.RemoveAll(lang => lang.Equals("Hindi", StringComparison.OrdinalIgnoreCase) || lang.Equals("English", StringComparison.OrdinalIgnoreCase));

                // Process remaining languages
                foreach (string lang in regionalLanguages)
                {
                    if (!uniqueLanguages.Contains(lang) && !restrictedLanguages.Contains(lang, StringComparer.OrdinalIgnoreCase))
                    {
                        Guid langId = language(service, lang);
                        if (langId != Guid.Empty)
                        {
                            AddUniqueLanguage(finalLanguageArray, lang, langId);
                            uniqueLanguages.Add(lang);
                        }
                    }
                }

                // Update zx_regionallanguagetext with standardized format
                bd["zx_regionallanguagetext"] = uniqueLanguages.Any() ? string.Join(";", uniqueLanguages) : "";
            }
            else if (uniqueLanguages.Any())
            {
                // If zx_regionallanguagetext is empty but we have languages, update it
                bd["zx_regionallanguagetext"] = string.Join(";", uniqueLanguages);
            }

            // Step 3: Update zx_regionallanguages with final language array
            if (finalLanguageArray.Count > 0)
            {
                bd["zx_regionallanguages"] = finalLanguageArray.ToString();
            }
            else
            {
                bd["zx_regionallanguages"] = "";
            }

            // Step 4: Process zx_commonlanguageforgeosplit field
            string commonLanguage = bd.Contains("zx_commonlanguageforgeosplit")
                ? bd.GetAttributeValue<string>("zx_commonlanguageforgeosplit")?.Trim()
                : string.Empty;

            if (!string.IsNullOrEmpty(commonLanguage))
            {
                // Standardize case
                string standardizedCommonLang = char.ToUpper(commonLanguage[0]) + commonLanguage.Substring(1).ToLower();

                // Only add if not restricted
                if (!restrictedLanguages.Contains(standardizedCommonLang, StringComparer.OrdinalIgnoreCase))
                {
                    if (!uniqueLanguages.Contains(standardizedCommonLang))
                    {
                        Guid langId = language(service, standardizedCommonLang);
                        if (langId != Guid.Empty)
                        {
                            AddUniqueLanguage(finalLanguageArray, standardizedCommonLang, langId);
                            uniqueLanguages.Add(standardizedCommonLang);
                        }
                    }
                    bd["zx_commonlanguageforgeosplit"] = standardizedCommonLang;
                }
                else
                {
                    bd["zx_commonlanguageforgeosplit"] = "";
                }
            }



            // Step 5: Create language creatives for unique languages and universal languages
            if (context.MessageName.ToLower() == "create")
            {
                // Create creatives for regional languages
                foreach (string lang in uniqueLanguages.ToList()) // Create a copy to avoid modification issues
                {
                    Guid langId = language(service, lang);
                    if (langId != Guid.Empty)
                    {
                        CreateLanguageCreatives(service, campaign, langId);
                    }
                }

                // Create creatives for Hindi if zx_hindiuniversal is populated
                if (hindiUniversalMarkets.Any())
                {
                    Guid hindiLangId = language(service, "Hindi");
                    if (hindiLangId != Guid.Empty)
                    {
                        CreateLanguageCreatives(service, campaign, hindiLangId);
                    }
                }

                // Create creatives for English if zx_englishuniversal is populated
                if (englishUniversalMarkets.Any())
                {
                    Guid englishLangId = language(service, "English");
                    if (englishLangId != Guid.Empty)
                    {
                        CreateLanguageCreatives(service, campaign, englishLangId);
                    }
                }
            }

            // Final update to ensure consistency
            bd["zx_regionallanguagetext"] = uniqueLanguages.Count > 0 ? string.Join(";", uniqueLanguages) : "";

            bd["zx_hindiuniversal"] = hindiUniversalMarkets.Any() ? string.Join("+", hindiUniversalMarkets) : "";
            bd["zx_englishuniversal"] = englishUniversalMarkets.Any() ? string.Join("+", englishUniversalMarkets) : "";
        }

        private EntityReference Method_UpdateMarkettypeLookUp(IOrganizationService service, OptionSetValue markettype, Entity bd, int zx_id)
        {
            string fetchXmlMarketMaster = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                <entity name='zx_markettypemaster'>
                    <attribute name='statecode'/>
                    <attribute name='zx_markettypemasterid'/>
                    <attribute name='zx_name'/>
                    <attribute name='zx_medium'/>
                    <attribute name='zx_markettype'/>
                    <filter type='and'>
                        <condition attribute='statecode' operator='eq' value='0'/>
                        <condition attribute='zx_markettype' operator='eq' value='{markettype.Value}'/>
                    </filter>
                    <link-entity name='zx_campaigntype' alias='aa' link-type='inner' from='zx_campaigntypeid' to='zx_medium'>
                        <filter type='and'>
                            <condition attribute='zx_id' operator='eq' value='{zx_id}'/>
                        </filter>
                    </link-entity>
                </entity>
            </fetch>";

            EntityCollection MarkettypeLookupRetrieve = service.RetrieveMultiple(new FetchExpression(fetchXmlMarketMaster));

            if (MarkettypeLookupRetrieve.Entities.Count == 0)
            {
                return null;
            }

            Entity marketTypeEntity = MarkettypeLookupRetrieve.Entities.FirstOrDefault();
            return new EntityReference("zx_markettypemaster", marketTypeEntity.Id);
        }

        private void AddUniqueLanguage(JArray languageArray, string name, Guid id)
        {
            if (string.IsNullOrEmpty(name) || id == Guid.Empty) return;
            if (!languageArray.Any(item => item["id"].ToString() == id.ToString()))
            {
                JObject languageObject = new JObject
                {
                    ["name"] = name,
                    ["id"] = id.ToString()
                };
                languageArray.Add(languageObject);
            }
        }

        private void CreateLanguageCreatives(IOrganizationService service, Entity campaign, Guid languageId)
        {
            /*
             if (!campaign.Contains("zx_campaigntype") || !campaign.Contains("zx_variant")) return;

             string creativeFetch = $@"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
                 <entity name='zx_creative'>
                     <attribute name='statecode'/>
                     <attribute name='zx_creativeid'/>
                     <attribute name='zx_creativename'/>
                     <attribute name='zx_ctv'/>
                     <attribute name='zx_landing_page_url'/>
                     <attribute name='zx_language_creative_url'/>
                     <attribute name='zx_creativeduration'/>
                     <attribute name='zx_language'/>
                     <attribute name='zx_variant'/>
                     <attribute name='zx_cta'/>
                     <attribute name='zx_medium'/>
                     <filter type='and'>
                         <condition attribute='statecode' operator='eq' value='0'/>
                         <condition attribute='zx_medium' operator='eq' value='{((EntityReference)campaign["zx_campaigntype"]).Id}'/>
                         <condition attribute='zx_variant' operator='eq' value='{((EntityReference)campaign["zx_variant"]).Id}'/>
                         <condition attribute='zx_language' operator='eq' value='{languageId}'/>
                     </filter>
                 </entity>
             </fetch>";

             EntityCollection creativeCollection = service.RetrieveMultiple(new FetchExpression(creativeFetch));

             foreach (Entity record in creativeCollection.Entities)
             {
                 Entity creativesheet = new Entity("zx_languagesandcreatives")
                 {
                     ["zx_language"] = new EntityReference("zx_creative_language", languageId),
                     ["zx_campaign"] = new EntityReference("zx_campaign", campaign.Id),
                     ["zx_creativenames"] = new EntityReference("zx_creative", record.Id)
                     ,
                      ["zx_cta"] = record.Contains("zx_cta") ? record["zx_cta"] : null,
                     ["zx_creativeduration"] = record.Contains("zx_creativeduration") ? record["zx_creativeduration"] : null,
                     ["zx_creativenames"] = new EntityReference("zx_creative", record.Id),
                     ["zx_languagecreativeurl"] = record.Contains("zx_language_creative_url") ? record["zx_language_creative_url"] : null,
                     ["zx_landingpageurl"] = record.Contains("zx_landing_page_url") ? record["zx_landing_page_url"] : null,
                     ["zx_campaign"] = new EntityReference("zx_campaign", campaign.Id)
                 };


                 string checkFetch = $@"<fetch version='1.0' mapping='logical' distinct='false'>
                     <entity name='zx_languagesandcreatives'>
                         <attribute name='zx_languagesandcreativesid'/>
                         <filter type='and'>
                             <condition attribute='zx_campaign' operator='eq' value='{campaign.Id}'/>
                             <condition attribute='zx_language' operator='eq' value='{languageId}'/>
                             <condition attribute='zx_creativenames' operator='eq' value='{record.Id}'/>
                         </filter>
                     </entity>
                 </fetch>";
            */

            if (!campaign.Contains("zx_campaigntype")) return;


            Entity creativesheet = new Entity("zx_languagesandcreatives")
            {
                ["zx_language"] = new EntityReference("zx_creative_language", languageId),
                ["zx_campaign"] = new EntityReference("zx_campaign", campaign.Id)
            };

            string checkFetch = $@"<fetch version='1.0' mapping='logical' distinct='false'>
                <entity name='zx_languagesandcreatives'>
                    <attribute name='zx_languagesandcreativesid'/>
                    <filter type='and'>
                        <condition attribute='zx_campaign' operator='eq' value='{campaign.Id}'/>
                        <condition attribute='zx_language' operator='eq' value='{languageId}'/>
                    </filter>
                </entity>
            </fetch>";

            if (service.RetrieveMultiple(new FetchExpression(checkFetch)).Entities.Count == 0)
            {
                service.Create(creativesheet);
            }
        }





        private string GetConcatenatedMarketFromCRM(IOrganizationService service, string marketName)
        {
            string fetchXml = $@"<fetch version='1.0' mapping='logical' distinct='true'>
                <entity name='zx_concatenatedmarket'>
                    <attribute name='zx_concatmarket'/>
                    <filter type='and'>
                        <condition attribute='statecode' operator='eq' value='0'/>
                        <condition attribute='zx_mashmarket' operator='like' value='%{marketName}%'/>
                    </filter>
                    <order attribute='createdon' descending='true'/>
                </entity>
            </fetch>";

            EntityCollection result = service.RetrieveMultiple(new FetchExpression(fetchXml));
            string concatMarket = result.Entities.Count > 0 ?
                result.Entities[0].GetAttributeValue<string>("zx_concatmarket") ?? marketName :
                marketName;

            string finalconcatmarketdigital = concatMarket;

            string fetchXmlEntireMarket = $@"
        <fetch version='1.0' mapping='logical' distinct='true'>
            <entity name='zx_markets'>
                <attribute name='zx_marketsid'/>
                <attribute name='zx_digitalmarkets'/>
                <attribute name='zx_digitalmarketmapping'/>
                <filter type='and'>
                    <condition attribute='statecode' operator='eq' value='0'/>
                    <filter type='or'>
                        <condition attribute='zx_digitalmarkets' operator='eq' value='{concatMarket}'/>
                        
                    </filter>
                </filter>
            </entity>
        </fetch>";

            EntityCollection entireResultMarket = service.RetrieveMultiple(new FetchExpression(fetchXmlEntireMarket));

            if (entireResultMarket.Entities.Count > 0)
            {


                var entity = entireResultMarket.Entities[0];
                finalconcatmarketdigital = entity.Contains("zx_digitalmarketmapping") && !string.IsNullOrEmpty(entity["zx_digitalmarketmapping"].ToString())
                   ? entity["zx_digitalmarketmapping"].ToString()
                   : entity["zx_digitalmarkets"].ToString();
            }



            return finalconcatmarketdigital;
        }
    }
}