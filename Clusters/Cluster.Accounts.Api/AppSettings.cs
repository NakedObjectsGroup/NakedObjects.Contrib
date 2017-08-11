using System.Collections.Generic;
using System.Configuration;


namespace Cluster.Accounts.Api
{
    /// <summary>
    /// Extracts standard app settings values from Web.config or App.config
    /// </summary>
    public static class AppSettings
    {
        private const string ClusterNamespace = "Cluster.Accounts.";

        public static string DefaultCurrencyCode() {          
            var appSetting = ConfigurationManager.AppSettings[ClusterNamespace + "DefaultCurrencyCode"];
            return appSetting ?? "USD";
        }
        
        public static string[] ValidCurrencyCodes() { 
            var appSetting = ConfigurationManager.AppSettings[ClusterNamespace + "ValidCurrencyCodes"];
            if (appSetting == null) return new string[] { "USD", "GBP" }; //Default set
            var codes = appSetting.Split(',');
            var output = new List<string>();
            foreach (string code in codes)
            {
                output.Add(code.Trim());
            }
            return output.ToArray();
        }
    }
}
