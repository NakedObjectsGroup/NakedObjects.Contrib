using System.Collections.Generic;
using System.Configuration;


namespace Cluster.Audit.Api
{
    /// <summary>
    /// Extracts standard app settings values from Web.config or App.config
    /// </summary>
    public static class AppSettings
    {
        private const string ClusterNamespace = "Cluster.Audit.";
        public static bool AuditQueryOnlyActions() { 
            string setting = ConfigurationManager.AppSettings[ClusterNamespace + "AuditQueryOnlyActions"];
            if (setting == null) return false;
            return bool.Parse(setting);
        }
    }
}
