using System.Configuration;


namespace Cluster.Countries.Api
{
    /// <summary>
    /// Extracts standard app settings values from Web.config or App.config
    /// </summary>
    public static class AppSettings
    {
        private const string ClusterNamespace = "Cluster.Countries.";

        public static string DefaultCountryISOCode() { 
            var appSetting = ConfigurationManager.AppSettings[ClusterNamespace + "DefaultCountryCode"]; 
            return appSetting ??  "USA";
        }
    }
}
