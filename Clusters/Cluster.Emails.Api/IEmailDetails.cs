using System.Net.Mail;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Emails.Api
{
    /// <summary>
    /// Result interface modelling an Email address
    /// </summary>
    public interface IEmailDetails : IDomainInterface
    {
        MailAddress AsMailAddress();
      
    }
}
