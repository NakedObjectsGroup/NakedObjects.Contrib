using System.Configuration;


namespace Cluster.Addresses.Impl
{
    /// <summary>
    /// Extracts standard app settings values from Web.config or App.config
    /// </summary>
    public static class AppSettings
    {
        private const string ClusterNamespace = "Cluster.Addresses.";

      public static string DefaultPostalAddressType() { 
          var appSetting = ConfigurationManager.AppSettings[ClusterNamespace + "DefaultPostalAddressType"];
          return appSetting ?? "Cluster.Addresses.Impl.UKAddress";
      }
    }
}
