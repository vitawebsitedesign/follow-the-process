using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace HubApp1
{
    public class Ann
    {
        public string Tkr { get; set; }
        public string CompanyName { get; set; }
        public bool PriceSens { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Gics { get; set; }

        public Ann(string tkr, bool priceSens, string title, string url)
        {
            bool validData = (tkr.Length == 3
                && title.Length > 0
                && url.Length > 0);
            if (!validData)
            {
                throw new ArgumentOutOfRangeException("Tried to create an Ann object, but the passed-in arguments to the constructor were out-of-range");
            }

            Tkr = tkr.ToUpper();
            PriceSens = priceSens;
            Title = title;
            Url = url;
        }

        public async Task<bool> SetGics()
        {
            // Cache gics
            if (!AppCache.IsCached(Tkr))
            {
                TkrData tkrData = await GetTkrData(Tkr);
                AppCache.Set(Tkr, tkrData.Gics);
            }

            Gics = AppCache.Get(Tkr).ToString();
            return true;
        }

        private async Task<TkrData> GetTkrData(string tkr)
        {
            JObject data = await Asx.TkrData(tkr);
            return new TkrData(data.ToString());
        }
    }
}