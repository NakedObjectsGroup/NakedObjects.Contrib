using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Audit.Api;
using Cluster.System.Api;
using Cluster.Users.Api;
using NakedObjects;

namespace App.Users.Test
{
    public class MockOrganisation : IUserOrg
    {
        public IUserService UserService { set; protected get; }

        public virtual int Id { get; set; }

        [Title]
        public virtual string Name { get; set; }

        public IUser AddUser(string userName)
        {
           return UserService.AddUser(userName, this);
        }

        public string ValidateAddUser(string userName)
        {
            var rb = new ReasonBuilder();
            string message = null;
            rb.AppendOnCondition(!UserService.CanAddUser(userName, this, out message) , message);
            return rb.Reason;
        }

        public IQueryable<IUser> ListUsers()
        {
            return UserService.ListUsersForOrganisation(this);
        }
    }
}
