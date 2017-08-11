using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Cluster.System.Api
{
    public interface IEmailSender
    {
        void SendEmail(MailMessage email);
    }
}
