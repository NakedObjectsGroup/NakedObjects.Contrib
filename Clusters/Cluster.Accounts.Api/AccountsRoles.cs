using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Cluster.Accounts.Api
{
    public static class AccountsRoles
    {
        public const string Accounting = "Accounting";

        public static bool IsAccounting(this IPrincipal principal)
        {
            return principal.IsInRole(Accounting);
        }
    }
}
