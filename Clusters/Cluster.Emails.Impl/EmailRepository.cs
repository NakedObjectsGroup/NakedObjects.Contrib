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
using System.ComponentModel;
using Cluster.Users.Api;

namespace Cluster.Emails.Impl
{
    [DisplayName("Emails")]
    public class EmailRepository 
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public IEmailSender EmailSender { set; protected get; }

        public IDocumentService DocumentService { set; protected get; }

        public IEmailService EmailService { set; protected get; }

        public IUserService UserService { set; protected get; }

        #endregion

        public IQueryable<IEmail> FindEmails(string toEmailAddress, [Optionally] DateTime? after = null, [Optionally] DateTime? before = null)
        {
            return EmailService.FindEmails(toEmailAddress, after, before);
        }
       
        public IQueryable<Email> AllEmails()
        {
            return Container.Instances<Email>();
        }

        #region Create Email
        public IEmail CreateEmail(
           IEmailAddressProvider recipient, 
           string fromEmailAddress,
           string subject, 
           [MultiLine(NumberOfLines=5)] string body, 
           [Optionally] IDocumentHolder associateAsDocumentWith)
       {
           var msg = new MailMessage(fromEmailAddress, recipient.DefaultEmailAddress().ToString(), subject, body);
           var email = EmailService.SendAndSaveEmail(msg);
           if (associateAsDocumentWith != null)
           {
               DocumentService.AddExternalDocument(associateAsDocumentWith, email);
           }
           return email;
       }
       
public string Validate1CreateEmail(string fromEmailAddress)
{
    return Helpers.ValidateEmailAddress(fromEmailAddress);
}
      
        public string Default1CreateEmail()
      {
          var userEmail = UserService.CurrentUser().EmailAddress;
          return userEmail ?? AppSettings.DefaultSender;
      }
 

        #endregion

    }
}
