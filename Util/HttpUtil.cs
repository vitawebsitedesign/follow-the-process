using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class HttpUtil
    {
        public static async Task<string> DoGetReq(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Tried to get asx data from a url, but failed");
            }
            return await res.Content.ReadAsStringAsync();
        }
    }
}