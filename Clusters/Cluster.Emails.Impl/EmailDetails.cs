using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Cluster.Emails.Api;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Emails.Impl
{
    public class EmailDetails : IEmailDetails, IUpdateableEntity
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
            t.Append(EmailAddress).Append(Description);
            return t.ToString();
        }
        #endregion

        #region Properties
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

 

        [MemberOrder(10)]
        public virtual string EmailAddress { get; set; }

        
public string ValidateEmailAddress(string addr)
{
    return Helpers.ValidateEmailAddress(addr);
}
      

        [NakedObjectsIgnore]
        public MailAddress AsMailAddress()
        {
            return new MailAddress(EmailAddress);
        }

        [StringLength(20), MemberOrder(20), Optionally]
        public virtual string Description { get; set; }

        [MemberOrder(30), DefaultValue(true)]
        public virtual bool Current { get; set; }

        [MemberOrder(40)]
        public virtual bool Verified { get; set; }

        [MemberOrder(50)]
        public virtual bool Preferred { get; set; }

                [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime LastModified { get; set; }
#endregion
    }
}
