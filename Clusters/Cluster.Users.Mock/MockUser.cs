using System.Linq;
using System.ComponentModel;
using Cluster.Users.Api;
using NakedObjects;
using System.Collections.Generic;
using System;
namespace Cluster.Users.Mock
{
    public class MockUser : IUser
    {
        public IDomainObjectContainer Container { set; protected get; }

        public override string ToString()
        {
            return UserName;
        }

        public virtual int Id { get; set; }

        public virtual string UserName { get; set; }

        [Optionally]
        public virtual string FullName { get; set; }

        public virtual string EmailAddress { get; set; }

        private ICollection<MockRole> _Roles = new List<MockRole>();

        [MemberOrder(20)]
        public virtual ICollection<MockRole> Roles
        {
            get
            {
                return _Roles;
            }
            set
            {
                _Roles = value;
            }
        }


        public string ListOfRoles
        {
            get { return null; }
        }

        public void AddRoles(params string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public void RemoveRoles(params string[] roleNames)
        {
            throw new NotImplementedException();
        }


        public IUserOrg Organisation
        {
            get { return null; }
        }


        public bool CanActFor(IUserOrg organisation)
        {
            throw new NotImplementedException();
        }
    }
}
