using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Cluster.Emails.Api
{
    public interface IEmailService
    {
        /// <summary>
        /// Returns a transient object
        /// </summary>
        /// <returns></returns>
        IEmailDetails CreateNewEmailDetails();

        IEmailDetails FindEmailDetailsById(int id);

        IQueryable<T> FindByEmailAddress<T>(string emailAddress) where T : class, IHasEmailDetails;

        IEmail FindEmailById(int id);

        IQueryable<IEmail> FindEmails(string toEmailAddress, DateTime? after, DateTime? before);  //TODO more params?

        IEmail  SendAndSaveEmail(MailMessage message);

        IEmail Save(MailMessage message);

    }
}
