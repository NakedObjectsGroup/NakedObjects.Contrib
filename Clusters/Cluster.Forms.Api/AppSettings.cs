using System.Collections.Generic;
using System.Configuration;


namespace Cluster.Forms.Api
{
    /// <summary>
    /// Extracts standard app settings values from Web.config or App.config
    /// </summary>
    public static class AppSettings
    {
        private const string ClusterNamespace = "Cluster.Forms.";

        /// <summary>
        /// The field label within a .pdf form that contains the FormCode (may be a hidden field) such that the
        /// FormDefinition can be looked up.
        /// </summary>
        /// <returns></returns>
        public static string FieldLabelForFormCode() {
            var appSetting = ConfigurationManager.AppSettings[ClusterNamespace + "FieldLabelForFormCode"];
            return appSetting ?? "FormCode";
        }

        public static string URLForFormSubmission()
        {
            var appSetting = ConfigurationManager.AppSettings[ClusterNamespace + "URLForFormSubmission"];
            return appSetting ?? "http://URLForFormSubmissionNotSpecifiedInWebConfig";
        }
    }
}
