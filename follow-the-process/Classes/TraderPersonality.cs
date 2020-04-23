using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubApp1
{
    public class TraderPersonality
    {
        public string Personality { get; set; }

        public TraderPersonality(string inPersonality)
        {
            Personality = inPersonality;
        }
    }
}