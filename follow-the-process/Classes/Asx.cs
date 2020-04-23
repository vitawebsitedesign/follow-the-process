using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Util;
using Windows.Data.Json;
using Windows.Storage;

namespace HubApp1
{
    public static class Asx
    {
        private static string _annPgUrl = @"http://www.asx.com.au/asx/statistics/prevBusDayAnns.do";
        private static string _asxDataUrl = @"http://data.asx.com.au/data/1/company/@TICKER@?fields=primary_share";
        private static string _asxDataUrlTkr = "@TICKER@";
        private static string _asxDataCompanyName = "name_full";
        private static string _asxDomain = @"www.asx.com.au";
        private static string _pattAnnTable = @"<table cellspacing=""0"" class=""contenttable announcements"">[\w\W]+</table>";
        private static string _pattAnnRows = @"<tr.*?>[\w\W]+?</tr>";
        private static string _pattAnnCells = @"<td[\w\W]*?>[\w\W]+?</td>";
        private static string _pattAnnUrl = @"/asx/statistics/displayAnnouncement.do\?display=pdf&idsId=[0-9]+";
        private static string _pattHtmlTags = @"<.*?>";
        private static string _priceSensHtml = @"<td class=""pricesens"">";
        private static string[] _blacklistedGics = "materials,energy".Split(',');
        private static string[] _blacklistedPhrases = "Appendix,Quarterly Activities Report,Quarterly,Cashflow,Report,Earnings Guidance,Financial Guidance,Annual General Meeting,cashflow report,admission to official list,address,debt,capital raising,merge,distribution,query,dividend,halt".Split(',');
        private static int _indexAnnTkr = 0;
        private static int _indexAnnTitle = 3;
        private static string _companyNameSentinel = "";

        #region public

        public static async Task<List<Ann>> PriceSensAnns()
        {
            try
            {
                string html = await GetAnnPgHtml();
                Match tableMatch = Regex.Match(html, _pattAnnTable);
                bool annsToday = (tableMatch.Length > 0);
                if (!annsToday)
                {
                    throw new InvalidDataException("Tried to get announcements, but found none");
                }

                // Get price-sensitive non-common anns
                List<string> annRowsRaw = AnnRows(tableMatch.Value);
                List<Ann> priceSensAnns = await FilterPriceSensAnns(annRowsRaw);

                // Filter gics
                priceSensAnns.RemoveAll(ann =>
                {
                    return BlacklistedAnnGics(ann.Gics);
                });

                // Get titles
                foreach (Ann ann in priceSensAnns)
                {
                    ann.CompanyName = await CompanyName(ann.Tkr);
                }

                return priceSensAnns;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<JObject> TkrData(string tkr)
        {
            JObject data = null;

            try
            {
                if (tkr.Length != 3)
                {
                    throw new ArgumentOutOfRangeException("invalid tkr: " + tkr);
                }

                string url = _asxDataUrl.Replace(_asxDataUrlTkr, tkr);
                string jsonStr = await HttpUtil.DoGetReq(url);
                data = JsonConvert.DeserializeObject<JObject>(jsonStr);
            }
            catch (Exception)
            {
            }

            return data;
        }

        #endregion

        #region private

        private static async Task<string> GetAnnPgHtml()
        {
            return await HttpUtil.DoGetReq(_annPgUrl);
        }

        private static bool BlacklistedAnnTitle(string annTitle)
        {
            return Blacklisted(annTitle, _blacklistedPhrases);
        }

        private static bool BlacklistedAnnGics(string annGics)
        {
            return Blacklisted(annGics, _blacklistedGics);
        }

        private static bool Blacklisted(string needle, string[] haystack)
        {
            foreach (string hay in haystack)
            {
                if (needle.Trim().ToLowerInvariant().Contains(hay.Trim().ToLowerInvariant()))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<string> AnnRows(string table)
        {
            MatchCollection rowMatches = Regex.Matches(table, _pattAnnRows);
            List<string> rows = new List<string>();
            bool header = true;

            foreach (Match rowMatch in rowMatches)
            {
                if (header)
                {
                    header = false;
                    continue;
                }
                rows.Add(rowMatch.Value);
            }

            return rows;
        }

        private static async Task<List<Ann>> FilterPriceSensAnns(List<string> rows)
        {
            List<Ann> priceSensAnns = new List<Ann>();
            foreach (string row in rows)
            {
                // Filter price-sensitive
                bool priceSens = (row.Contains(_priceSensHtml));
                if (priceSens)
                {
                    // Filter title
                    Ann ann = await AnnObj(row);
                    if (!BlacklistedAnnTitle(ann.Title))
                    {
                        priceSensAnns.Add(ann);
                    }
                }
            }

            return priceSensAnns;
        }

        private static async Task<Ann> AnnObj(string row)
        {
            MatchCollection matches = Regex.Matches(row, _pattAnnCells);
            List<string> annSegments = new List<string>();

            foreach (Match cellMatch in matches)
            {
                if (cellMatch.Value != null && cellMatch.Value.Length > 0)
                {
                    annSegments.Add(StripTags(cellMatch.Value));
                }
            }

            string tkr = annSegments[_indexAnnTkr].Trim();
            string title = annSegments[_indexAnnTitle].Trim();
            string url = AnnUrl(row).Trim();
            Ann ann = new Ann(tkr, true, title, url);
            await ann.SetGics();
            return ann;
        }

        private static string AnnUrl(string row)
        {
            Match urlMatch = Regex.Match(row, _pattAnnUrl);
            if (urlMatch.Length == 0)
            {
                throw new Exception("Tried to get announcement pdf url, but failed");
            }
            return _asxDomain + urlMatch.Value;
        }

        private static string StripTags(string html)
        {
            return Regex.Replace(html, _pattHtmlTags, string.Empty);
        }

        private static async Task<string> CompanyName(string tkr)
        {
            if (tkr.Length != 3)
            {
                return _companyNameSentinel;
            }

            JToken companyName;
            JObject data = await TkrData(tkr);

            if (data.TryGetValue(_asxDataCompanyName, out companyName))
            {
                return companyName.Value<string>();
            }
            return _companyNameSentinel;
        }

        #endregion
    }
}