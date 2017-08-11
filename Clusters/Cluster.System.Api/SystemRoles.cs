using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Cluster.System.Api
{
    public static class SystemRoles
    {
        public const string SysAdmin = "System Administrator";

        public static bool IsSysAdmin(this IPrincipal principal)
        {
            return principal.IsInRole(SysAdmin); 
        }

        public const string Developer = "Developer";

        public static bool IsDeveloper(this IPrincipal principal)
        {
            return principal.IsInRole(Developer);
        }
    }
}
