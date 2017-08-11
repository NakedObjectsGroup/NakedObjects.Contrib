using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Cluster.Audit.Api;
using Cluster.System.Api;
using NakedObjects;
using NakedObjects.Security;

namespace Cluster.Audit.Impl
{
    public class AuditAuthorizer : INamespaceAuthorizer
    {
        public IDomainObjectContainer Container { set; protected get; }


        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {

            throw new DomainException("No properties in the Cluster.Audit space should be editable!");
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return principal.IsAuditor() || principal.IsSysAdmin();  
        }

        public string NamespaceToAuthorize
        {
            get { return this.GetType().Namespace ; }
        }
    }
}