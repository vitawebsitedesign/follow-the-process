using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubApp1
{
    public class Chart
    {
        private string _urlTemplate = @"http://bigcharts.marketwatch.com/kaavio.Webhost/charts/big.chart?nosettings=1&symb=au%3a@TKR&uf=0&type=4&size=4&style=320&freq=1&time=9&rand=180992540&compidx=aaaaa%3a0&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=635&width=1045&mocktick=1";
        private string _urlTemplateTkrSeg = "@TKR";
        public string Tkr { get; set; }
        public string Url { get; set; }

        public Chart(string tkr)
        {
            Tkr = tkr.ToUpper();
            Url = _urlTemplate.Replace(_urlTemplateTkrSeg, tkr.ToUpperInvariant());
        }
    }
}
