using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Util;

namespace HubApp1
{
    public static class HotCopper
    {
        private static string _frontPageUrl = @"https://hotcopper.com.au/";
        private static string _pattTable = @"<h3>Most discussed<\/h3>[\w\W]*<div class='panel-content'>[\w\W]+?<\/div>";
        private static string _pattTableRow = @"<span class='tag-pill symbol asx [A-Za-z0-9]{3}'>[A-Z0-9]{3}<\/span>";
        private static string _pattTableRowTkr = @"[A-Z0-9]{3}";

        public static async Task<List<string>> GetMostDiscussedTkrs()
        {
            try
            {
                // Get front page html
                string html = await GetFrontPageHtml();
                // Extract "most discussed" table
                string table = GetTable(html);
                // Extract rows from table
                List<string> rows = GetTableRows(table);
                // Extract tkrs from ea row
                List<string> mostDiscussedTkrs = GetTableRowTkrs(rows);
                // Return list of tkrs
                return mostDiscussedTkrs;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static async Task<string> GetFrontPageHtml()
        {
            return await HttpUtil.DoGetReq(_frontPageUrl);
        }

        private static string GetTable(string html)
        {
            Match tableMatch = Regex.Match(html, _pattTable);
            if (tableMatch.Length == 0)
            {
                throw new Exception("Failed to get table from front page");
            }

            return tableMatch.Value;
        }

        private static List<string> GetTableRows(string table)
        {
            MatchCollection rowsMatched = Regex.Matches(table, _pattTableRow);
            if (rowsMatched.Count == 0)
            {
                throw new Exception("Failed to get rows from table");
            }

            return rowsMatched.OfType<Match>()
                .Select(r => r.Value)
                .ToList();
        }

        private static List<string> GetTableRowTkrs(List<string> rows)
        {
            List<string> tkrs = new List<string>();

            foreach (string row in rows)
            {
                Match tkrMatch = Regex.Match(row, _pattTableRowTkr);
                if (tkrMatch.Length == 0)
                {
                    throw new Exception("Failed to get tkr from row");
                }

                tkrs.Add(tkrMatch.Value);
            }

            return tkrs;
        }
    }
}
