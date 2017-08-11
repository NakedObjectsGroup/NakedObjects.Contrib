using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Cluster.Tasks.Api;
using Cluster.Users.Api;
using NakedObjects;
using NakedObjects.Security;

namespace Cluster.Tasks.Impl
{
    public class TasksAuthorizer : INamespaceAuthorizer
    {
        public IDomainObjectContainer Container { set; protected get; }


        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {

            return principal.IsTaskAssignee();  
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return principal.IsTaskAssignee();  
        }

        public string NamespaceToAuthorize
        {
            get { return this.GetType().Namespace ; }
        }
    }
}