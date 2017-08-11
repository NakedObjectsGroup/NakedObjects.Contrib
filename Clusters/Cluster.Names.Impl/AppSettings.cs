using System.Configuration;

namespace Cluster.Names.Impl
{
    /// <summary>
    /// Extracts standard app settings values from Web.config or App.config
    /// </summary>
    public static class AppSettings
    {
        private const string ClusterNamespace = "Cluster.Names.";

        public static string DefaultNameType() {            
            var appSetting = ConfigurationManager.AppSettings[ClusterNamespace +"DefaultNameType"];
            return appSetting ?? "Cluster.Names.Impl.WesternName";
        }
    }
}
