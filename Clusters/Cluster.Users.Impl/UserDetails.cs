using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cluster.Users.Api;
using Cluster.System.Api;
using NakedObjects;
using System.Text;
using Cluster.Emails.Api;
using System.Net.Mail;
using NakedObjects.Services;

namespace Cluster.Users.Impl
{
    //This class may be considered a 'wrapper' for an underlying Microsoft IdentityUser.  This
    //wrapper will exist for all users that have been given a Poster role.
    public class UserDetails : Cluster.Users.Api.IUser, IEmailAddressProvider
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public UserService UserService { set; protected get; }

        public IClock Clock { set; protected get; }

        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }
        #endregion

        #region LifeCycle methods
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
            return FullName;
        }
        #endregion

         #region Properties
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(10)]
        public string UserName { get; set; }

        [MemberOrder(20), Optionally()]  
        public virtual string FullName { get; set; }

        #region EmailAddress
        [MemberOrder(30)]
        public string EmailAddress { get; set; }

        //Implementation of IEmailAddressProvider
        [NakedObjectsIgnore]
        public MailAddress DefaultEmailAddress()
        {
            return new MailAddress(EmailAddress);
        }
        #endregion

        #region Organisations Collection of type IUserOrg


        private ICollection<UserOrganisationLink> _Organisation = new List<UserOrganisationLink>();

        [NakedObjectsIgnore]
        public virtual ICollection<UserOrganisationLink> OrganisationLinks
        {
            get
            {
                return _Organisation;
            }
            set
            {
                _Organisation = value;
            }
        }

        [MemberOrder(1030)]
        public void AddOrganisation(IUserOrg organisation)
        {
            PolymorphicNavigator.AddLink<UserOrganisationLink, IUserOrg, UserDetails>(organisation, this);
        }


          [MemberOrder(1040)]
        public void RemoveOrganisation(IUserOrg organisation)
        {
            PolymorphicNavigator.RemoveLink<UserOrganisationLink, IUserOrg, UserDetails>(organisation, this);
        }

        /// <summary>
        /// This is an optional, derrived collection, which shows the associated objects directly.
        /// It is more convenient for the user, but each element is resolved separately, so more
        /// expensive in processing terms.  Use this pattern only on smaller collections.
        /// </summary>
        [NotPersisted, MemberOrder(110)]
        public ICollection<IUserOrg> Organisations
        {
            get
            {
                return OrganisationLinks.Select(x => x.AssociatedRoleObject).ToList();
            }
        }


        [NakedObjectsIgnore]
        public bool CanActFor(IUserOrg organisation)
        {
           //TODO: Make more efficient by searching on links
            string typeCode = PolymorphicNavigator.GetType(organisation);
            int orgId = organisation.Id;
            return OrganisationLinks.SingleOrDefault(x => x.AssociatedRoleObjectId == orgId && x.AssociatedRoleObjectType == typeCode) != null;
        }
        #endregion

        [ConcurrencyCheck, Disabled, MemberOrder(1000)]
        public virtual DateTime? LastModified { get; set; }
        #endregion

        #region Roles
        public void AddRole(string role)
        {
           if (string.IsNullOrEmpty(Roles)) {
                Roles = role;
            } else
            {
                Roles += ", " + role;
            }
        }

        [Disabled]
        public string Roles { get; set; }

        #endregion
    }
}
