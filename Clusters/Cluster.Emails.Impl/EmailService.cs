using NakedObjects;
using System.Linq;
using System.Collections.Generic;
using System;
using NakedObjects.Util;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Cluster.Emails.Api;
using System.Net.Mail;
using Cluster.System.Api;
using Cluster.Documents.Api;

namespace Cluster.Emails.Impl
{
    public class EmailService : IEmailService
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public IEmailSender EmailSender { set; protected get; }

        public IDocumentService DocumentService { set; protected get; }

        #endregion

        public IEmailDetails CreateNewEmailDetails()
        {
            return Container.NewTransientInstance<EmailDetails>();
        }


        public IEmailDetails FindEmailDetailsById(int telephoneId)
        {
            return Container.Instances<EmailDetails>().Single(x => x.Id == telephoneId);
        }

        public IQueryable<EmailDetails> FindEmailDetails(string matching)
        {
            return Container.Instances<EmailDetails>().Where(x => x.EmailAddress.ToUpper().Contains(matching));
        }


        public IQueryable<T> FindByEmailAddress<T>(string emailAddress) where T : class, IHasEmailDetails
        {
            var emailDetails = FindEmailDetails(emailAddress);
            var targets = Container.Instances<T>();

            return from emailDetail in emailDetails
                   from t in targets
                   where t.EmailDetailsId == emailDetail.Id
                   select t;
        }

        public IEmail FindEmailById(int id)
        {
            return Container.Instances<Email>().Where(e => e.Id == id).Single();
        }

        public IEmail SendAndSaveEmail(MailMessage message)
        {
            EmailSender.SendEmail(message);
            return Save(message);
        }

        public IEmail Save(MailMessage message)
        {
            var email = Container.NewTransientInstance<Email>();
            email.PopulateFrom(message);
            Container.Persist(ref email);
            return email;
        }

        public IQueryable<IEmail> FindEmails(string toEmailAddress, [Optionally] DateTime? after = null, [Optionally] DateTime? before = null)
        {
            var q = from e in Container.Instances<Email>()
                    where e.To.ToUpper().Contains(toEmailAddress.ToUpper())
                    select e;
            return q;
            //TODO: Implement date testing
        }
    }
}
