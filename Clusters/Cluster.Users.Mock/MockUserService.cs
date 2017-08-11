using System.Linq;
using System.ComponentModel;
using Cluster.Users.Api;
using NakedObjects;
using System;
using System.Collections.Generic;

namespace Cluster.Users.Mock
{
    [DisplayName("Users")]
    public class MockUserService : IUserService
    {
        public IDomainObjectContainer Container { set; protected get; }


        #region FindUser

        public IUser FindUserByUserName(string name)
        {
            return Container.Instances<MockUser>().Where(x => x.UserName.ToUpper() == name.ToUpper()).SingleOrDefault();
        }

        public IQueryable<IUser> FindUsersByRealOrUserName(string nameMatch)
        {
            throw new NotImplementedException();
            //return from obj in Container.Instances<MockUser>()
            //       where obj.UserName.ToUpper().Contains(nameMatch.ToUpper())
            //       select obj;
        }
        #endregion

        #region CurrentUser
        [NakedObjectsIgnore]
        IUser IUserService.CurrentUser()
        {
            return CurrentUser();
        }

        [DisplayName("Me")]
        public MockUser CurrentUser()
        {
            var userName = Container.Principal.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                userName = Constants.UNKNOWN;
            }
            return Container.Instances<MockUser>().Single(x => x.UserName == userName);
        }

        #endregion


        public IUser FindUserById(int id)
        {
            return Container.Instances<MockUser>().Single(x => x.Id == id);
        }


        public bool CurrentUserHasRole(string roleName)
        {
            throw new NotImplementedException();
        }


        public IUser FindById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IUser> FindUser(string match, string withRoleName = null)
        {
            throw new NotImplementedException();
        }

        public bool CanAddUser(string userName, IUserOrg toOrg, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public IUser AddUser(string userName, IUserOrg toOrg)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IUser> ListUsersForOrganisation(IUserOrg organisation)
        {
            throw new NotImplementedException();
        }


        public bool CurrentUserCanActFor(IUserOrg organisation)
        {
            throw new NotImplementedException();
        }

        public ICollection<IUserOrg> OrganisationsOfCurrentUser() {
            throw new NotImplementedException();
        }
    }
}
