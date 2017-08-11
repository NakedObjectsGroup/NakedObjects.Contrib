using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using NakedObjects;
using NakedObjects.Value;
using Cluster.System.Api;
using System.ComponentModel.DataAnnotations;

namespace Cluster.Emails.Impl
{
    public class EmailAttachment :IUpdateableEntity
    {
        #region Injected Services

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
            t.Append("Email Attachment");
            return t.ToString();
        }
        #endregion

        #region Properties
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual FileAttachment Attachment
        {
            get
            {
                if (AttContent == null) return null;
                return new FileAttachment(AttContent, AttName, AttMime) { DispositionType = "inline" };
            }
        }

        [Hidden]
        public virtual byte[] AttContent { get; set; }

        [Hidden]
        public virtual string AttName { get; set; }

        [Hidden]
        public virtual string AttMime { get; set; }

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }

#endregion

        internal void AddOrChangeAttachment(FileAttachment newAttachment)
        {
            AttContent = newAttachment.GetResourceAsByteArray();
            AttName = newAttachment.Name;
            AttMime = newAttachment.MimeType;
        }


        internal void PopulateFrom(Attachment mailAtt)
        {
            AttMime = mailAtt.ContentType.Name;
            AttName = mailAtt.Name;
            AttContent = ReadFully(mailAtt.ContentStream);

        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        internal Attachment GetAttachment()
        {
            throw new NotImplementedException();
            //var attachment = new Attachment(new MemoryStream(AttContent), AttName)
            //{
            //    ContentType = ContentType.GetContentType(),
            //    Name = Name,
            //    TransferEncoding = TransferEncoding,
            //    NameEncoding = NameEncoding,
            //};

            //ContentDisposition.CopyTo(attachment.ContentDisposition);

            //return attachment;
        }  
    }
}
