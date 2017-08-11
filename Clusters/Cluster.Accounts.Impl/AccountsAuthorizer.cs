using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Cluster.Users.Api;
using Cluster.System.Api;
using Cluster.Accounts.Api;
using NakedObjects.Security;

namespace Cluster.Accounts.Impl
{
    public class AccountsAuthorizer : INamespaceAuthorizer
    {
        #region Injected Services
        public IUserService UserService { set; protected get; }
        #endregion

        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return IsVisible(principal, target, memberName);
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            if (principal.IsSysAdmin()) return true;
            if (principal.IsAccounting()) return true;

            if (typeof(CustomerAccount).IsAssignableFrom(target.GetType()))
            {
                CustomerAccount ca = target as CustomerAccount;
                if (!UserService.CurrentUserCanActFor(ca.AccountHolder)) return false;

                return true;
            }
            return false;
        }

        public string NamespaceToAuthorize
        {
            get { return this.GetType().Namespace; }
        }
    }
}
