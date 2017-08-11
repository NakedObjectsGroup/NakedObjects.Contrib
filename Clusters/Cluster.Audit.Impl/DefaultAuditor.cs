using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using NakedObjects.Audit;

namespace Cluster.Audit.Impl
{
    /// <summary>
    /// Can be registered as a Default Authorizer to handle all namespaces. Delegates to injected AuditEventRepository
    /// </summary>
    public class DefaultAuditor : IAuditor
    {
        #region Injected Services
        public AuditService AuditService { set; protected get; }
        #endregion

        public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters)
        {
            AuditService.ObjectActionInvoked(byPrincipal, actionName, onObject, queryOnly, withParameters);
        }

        public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters)
        {
            AuditService.ServiceActionInvoked(byPrincipal, actionName, serviceName, queryOnly, withParameters);
        }

        public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject)
        {
            AuditService.ObjectUpdated(byPrincipal, updatedObject);
        }

        public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject)
        {
            AuditService.ObjectPersisted(byPrincipal, updatedObject);
        }
    }
}
