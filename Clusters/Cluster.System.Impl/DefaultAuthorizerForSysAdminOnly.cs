using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Cluster.System.Api;
using NakedObjects.Security;

namespace Cluster.System.Impl
{
    public class DefaultAuthorizerForSysAdminOnly : ITypeAuthorizer<object>
    {
        public void Init()
        {
            //Not needed
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return principal.IsSysAdmin();
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return principal.IsSysAdmin();
        }

        public void Shutdown()
        {
            //Not needed
        }
    }
}