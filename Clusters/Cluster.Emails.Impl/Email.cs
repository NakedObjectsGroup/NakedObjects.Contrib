using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Cluster.Emails.Api;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Emails.Impl
{
    /// <summary>
    /// Mimics the structure of Microsoft's MailMessage class, but in a form that can be persisted.
    /// </summary>
    [Immutable(WhenTo.OncePersisted)]
    public class Email : IEmail, IUpdateableEntity
    {
        #region Injected Services
        public EmailRepository EmailService { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }

        public IClock Clock { set; protected get; }
        #endregion
        #region LifeCycle Methods
        public void Persisting()
        {
            LastModified = Clock.Now();
        }

        public void Updating()
        {
            LastModified = Clock.Now();
        }
        #endregion
        #region Title
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("Email");
            return t.ToString();
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(10)]
        public virtual string To { get; set; }

         [MemberOrder(12)]
        public virtual string CC { get; set; }

         [MemberOrder(14)]
        public virtual string Bcc { get; set; }

         [MemberOrder(15)]
        public virtual string ReplyToList { get; set; }


        [Hidden]
        public virtual bool IsBodyHtml { get; set; }
         [MemberOrder(20)]
        public virtual string From { get; set; }
         [MemberOrder(22)]
        public virtual string Sender { get; set; }

        [Title]
        [MemberOrder(30)]
        public virtual string Subject { get; set; }
         [MemberOrder(40), MultiLine(NumberOfLines=5)]
        public virtual string Body { get; set; }
        //public virtual Encoding BodyEncoding { get; set; }
        //public virtual Encoding SubjectEncoding { get; set; }

         [MemberOrder(50)]
        public virtual DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }
       // public virtual NameValueCollection Headers { get; set; }

         [MemberOrder(60)]
        public virtual MailPriority Priority { get; set; }


        //IList<SerializeableAlternateView> AlternateViews { get; set; }

        #region Attachments (collection)
        private ICollection<EmailAttachment> _Attachments = new List<EmailAttachment>();

        public virtual ICollection<EmailAttachment> Attachments
        {
            get
            {
                return _Attachments;
            }
            set
            {
                _Attachments = value;
            }
        }

        public void AddToAttachments(EmailAttachment value)
        {
            if (!(_Attachments.Contains(value)))
            {
                _Attachments.Add(value);
            }
        }

        public void RemoveFromAttachments(EmailAttachment value)
        {
            if (_Attachments.Contains(value))
            {
                _Attachments.Remove(value);
            }
        }

        public IList<EmailAttachment> Choices0RemoveFromAttachments()
        {
            return _Attachments.ToList();
        }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
        #endregion


        [NakedObjectsIgnore]
        public void PopulateFrom(MailMessage mailMessage)
        {
            //AlternateViews = new List<SerializeableAlternateView>();

            //Attachments = new List<SerializeableAttachment>();

            IsBodyHtml = mailMessage.IsBodyHtml;
            Body = mailMessage.Body;
            Subject = mailMessage.Subject;
            From = mailMessage.From.ToString();
            if (mailMessage.Sender != null)
            {
                Sender = mailMessage.Sender.ToString();
            }
            if (mailMessage.To.Count > 0)
            {
                To = mailMessage.To.ToString();
            }
            if (mailMessage.CC.Count > 0)
            {
                CC = mailMessage.CC.ToString();
            }
            if (mailMessage.Bcc.Count > 0)
            {
                Bcc = mailMessage.Bcc.ToString();
            }
            if (mailMessage.ReplyToList.Count > 0)
            {
                ReplyToList = mailMessage.ReplyToList.ToString();
            }

            foreach (Attachment att in mailMessage.Attachments)
            {
                var emailAtt = Container.NewTransientInstance<EmailAttachment>();
                emailAtt.PopulateFrom(att);
                Container.Persist(ref emailAtt);
                Attachments.Add(emailAtt);
            }
            //BodyEncoding = mailMessage.BodyEncoding;

            DeliveryNotificationOptions = mailMessage.DeliveryNotificationOptions;
            //Headers = new NameValueCollection(mailMessage.Headers);
            Priority = mailMessage.Priority;

            // SubjectEncoding = mailMessage.SubjectEncoding;

            //foreach (AlternateView av in mailMessage.AlternateViews)
            //{
            //    AlternateViews.Add(new SerializeableAlternateView(av));
            //}
        }

        [NakedObjectsIgnore]
        public MailMessage GetMailMessage()
        {
            var mailMessage = new MailMessage()
            {
                IsBodyHtml = IsBodyHtml,
                Body = Body,
                Subject = Subject,
                //BodyEncoding = BodyEncoding,
                DeliveryNotificationOptions = DeliveryNotificationOptions,
                Priority = Priority,
                //SubjectEncoding = SubjectEncoding,
            };
            mailMessage.From = new MailAddress(From);
            if (Sender != null)
            {
                mailMessage.Sender = new MailAddress(Sender);
            }
            mailMessage.To.Add(To);
            if (CC != null)
            {
                mailMessage.CC.Add(CC);
            }
            if (Bcc != null)
            {
                mailMessage.Bcc.Add(Bcc);
            }
            if (ReplyToList != null)
            {
                mailMessage.ReplyToList.Add(ReplyToList);
            }

            foreach (var attachment in Attachments)
            {
                mailMessage.Attachments.Add(attachment.GetAttachment());
            }

            //Headers.CopyTo(mailMessage.Headers);

            //foreach (var alternateView in AlternateViews)
            //{
            //    mailMessage.AlternateViews.Add(alternateView.GetAlternateView());
            //}

            return mailMessage;
        }

 
    }
}



