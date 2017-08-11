using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Cluster.Emails.Api
{
    public static class Helpers
    {
        public static string ValidateEmailAddress(string addr)
        {
            try
            {
                new MailAddress(addr);
                return null;
            }
            catch (Exception)
            {
                return "Invalid Email Address";
            }
        }
    }
}
