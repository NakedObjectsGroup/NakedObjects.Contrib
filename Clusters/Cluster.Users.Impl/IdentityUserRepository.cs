using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NakedObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Users.Impl
{
    public class IdentityUserRepository
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public IdentityUser FindIdentityUser(string userName)
        {
            string match = userName.Trim().ToUpper();
            return Container.Instances<IdentityUser>().FirstOrDefault(iu => iu.UserName.ToUpper() == match);
        }
      
        public IQueryable<IdentityUser> AllIdentityUsers()
        {
            return Container.Instances<IdentityUser>();
        }

        public IQueryable<IdentityRole> AllIdentityRoles()
        {
            return Container.Instances<IdentityRole>();
        }
     
        public IdentityRole CreateNewIdentityRole(string roleName)
        {
            IdentityRole role = Container.NewTransientInstance<IdentityRole>();
            role.Name = roleName;
            Container.Persist(ref role);
            return role;
        }
        

    }
}
