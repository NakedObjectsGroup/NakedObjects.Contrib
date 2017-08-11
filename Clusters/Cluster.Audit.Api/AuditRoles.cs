using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Cluster.Audit.Api
{
    public static class AuditRoles
    {
        public const string Auditor = "Auditor";

        public static bool IsAuditor(this IPrincipal principal)
        {
            return principal.IsInRole(Auditor);
        }
    }
}
