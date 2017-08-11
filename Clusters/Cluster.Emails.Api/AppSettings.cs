using System.Collections.Generic;
using System.Configuration;


namespace Cluster.Emails.Api
{
    /// <summary>
    /// Extracts standard app settings values from Web.config or App.config
    /// </summary>
    public static class AppSettings
    {
        private const string ClusterNamespace = "Cluster.Emails";

        public static string DefaultSender = ConfigurationManager.AppSettings[ClusterNamespace + "DefaultSender"];

        public static string SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        public static int SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        public static string SmtpUserName = ConfigurationManager.AppSettings["SmtpUserName"];
        public static string SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
    }
}
