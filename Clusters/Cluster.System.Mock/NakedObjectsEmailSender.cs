using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Cluster.System.Api;

namespace Cluster.System.Mock
{
    /// <summary>
    /// This class is included as a code illustration only.
    /// </summary>
    public class NakedObjectsEmailSender : IEmailSender
    {   
        public virtual void SendEmail(MailMessage email)
        {
            var client = new SmtpClient { Credentials = new NetworkCredential("licensesender@nakedobjects.net", "pong450"), Port = 25, Host = "auth.smtp.1and1.co.uk" };
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
