using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HubApp1
{
    public static class AppCache
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static bool IsCached(string key)
        {
            return (localSettings.Values[key] != null);
        }

        public static object Get(string key)
        {
            return localSettings.Values[key];
        }

        public static void Set(string key, object val)
        {
            localSettings.Values[key] = val;
        }
    }
}