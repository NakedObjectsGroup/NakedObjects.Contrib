using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Cluster.System.Api;
using Cluster.Users.Api;
using NakedObjects;
using NakedObjects.Security;
using NakedObjects.Util;

namespace Cluster.Users.Impl
{
    public class UsersAuthorizer : INamespaceAuthorizer
    {

       public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            if (principal.IsSysAdmin()) return true;
            if (typeof(UserDetails).IsAssignableFrom(target.GetType())) {
                var user = target as UserDetails;
                if (TypeUtils.IsPropertyMatch<UserDetails, string>(user, memberName, x => x.FullName) 
                    || TypeUtils.IsPropertyMatch<UserDetails, string>(user, memberName, x => x.EmailAddress))
                {                  
                    return SharesTargetsUserName(principal, target);
                }
            }
            //if (typeof(Role).IsAssignableFrom(target.GetType()))
            //{
            //    throw new DomainException("Role should be an immutable object"); //Because Role should be immutable.
            //}
            return false; //Safety net
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            if (principal.IsSysAdmin()) return true;
            if (typeof(UserDetails).IsAssignableFrom(target.GetType()))
            {
                if (memberName == "AddToRoles" || memberName == "RemoveFromRoles") return false;
                return SharesTargetsUserName(principal, target);
            }
            if (typeof(UserService).IsAssignableFrom(target.GetType()))
            {
                if (memberName == "CurrentUser") return true;
            }

            //Note: Role:  no reason to see anything except title.
            return false;
        }

        public string NamespaceToAuthorize
        {
            get { return this.GetType().Namespace ; }
        }

        private bool SharesTargetsUserName(IPrincipal principal, object target)
        {
            string targetUserName = (target as UserDetails).UserName;
            return principal.Identity.Name.ToUpper() == targetUserName.ToUpper();
        }
    }
}