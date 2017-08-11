using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Cluster.System.Api;
using System.Configuration;
using Cluster.Emails.Api;

namespace Cluster.Emails.Impl
{
    /// <summary>
    /// Extracts necessary details from Cluster.Emails.Api.AppSettings
    /// </summary>
    public class SimpleSmtpEmailSender : IEmailSender
    {
        public virtual void SendEmail(MailMessage email)
        {
            var client = new SmtpClient
            {
                Credentials = new NetworkCredential(AppSettings.SmtpUserName, AppSettings.SmtpPassword),
                Port = AppSettings.SmtpPort,
                Host = AppSettings.SmtpHost
            };
            try
            {
                client.Send(email);
            }
            catch (Exception e)
            {
                throw new Exception("Error sending mail message\n", e);
            }
        }
    }
}
