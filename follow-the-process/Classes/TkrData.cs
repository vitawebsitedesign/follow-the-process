using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubApp1
{
    public class TkrData
    {
        private string _tkrKey = "code";
        private string _gicsKey = "industry_group_name";
        public string Tkr { get; set; }
        public string Gics { get; set; }

        public TkrData(string jsonStr)
        {
            JToken tkrJObj = null;
            JToken gicsJObj = null;
            JObject json = JsonConvert.DeserializeObject<JObject>(jsonStr);

            // Convert string into object
            bool validData = (json.TryGetValue(_tkrKey, out tkrJObj) && json.TryGetValue(_gicsKey, out gicsJObj));
            if (!validData)
            {
                throw new Exception("Tried to convert tjr data json string into .NET object, but failed");
            }

            // Get data from object
            Tkr = tkrJObj.Value<string>();
            Gics = gicsJObj.Value<string>();
            if (Tkr == null || Gics == null)
            {
                throw new Exception("Tried to get tkr and gics from ASX data API, but failed");
            }

            bool enoughData = (Tkr != null && Gics != null);
            if (!enoughData)
            {
                throw new Exception("Tried to get tkr data from the json string, but the data wasnt there");
            }
        }
    }
}