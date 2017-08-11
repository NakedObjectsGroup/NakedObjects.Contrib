using System.Collections.Generic;
using System.Net.Mail;
using Cluster.System.Api;

namespace Cluster.Emails.Api
{
    // Role interface implemented by any class that can return one or more PostalAddresses
    public interface IEmailAddressProvider : IDomainInterface
    {
        MailAddress DefaultEmailAddress();
    }
}
