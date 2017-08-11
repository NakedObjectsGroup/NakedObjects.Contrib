using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Cluster.Emails.Api;
using NakedObjects;

namespace Cluster.Emails.Test
{
    public class TestPerson : IHasEmailDetails
    {     
        public virtual int Id { get; set; }
      
        public IEmailService EmailService { set; protected get; }


        #region EmailAddress Property of type IEmailDetails ('Result' interface)

        [Hidden()]
        public virtual int EmailDetailsId { get; set; }


        private IEmailDetails _EmailDetails;

        [NotPersisted()]
        public IEmailDetails EmailDetails
        {
            get
            {
                if (_EmailDetails == null && EmailDetailsId > 0)
                {
                    _EmailDetails = EmailService.FindEmailDetailsById(EmailDetailsId);
                }
                return _EmailDetails;
            }
            set
            {
                _EmailDetails = value;
                if (value == null)
                {
                    EmailDetailsId = 0;
                }
                else
                {
                    EmailDetailsId = value.Id;
                }
            }

        }
        #endregion


        public MailAddress DefaultEmailAddress()
        {
            return EmailDetails.AsMailAddress();
        }
   
    }

    public class TestPersonRepository
    {
        public IDomainObjectContainer Container { set; protected get; }

        public IEmailService EmailService { set; protected get; }

        public IQueryable<TestPerson> FindByEmailAddress(string matching)
        {
            return EmailService.FindByEmailAddress<TestPerson>(matching);
        }

        
        public IQueryable<TestPerson> AllTestPersons()
        {
            return Container.Instances<TestPerson>();
        }
      
    }
}
