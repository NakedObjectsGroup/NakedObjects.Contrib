using System;
using System.Collections.Generic;
using System.Linq;

using NakedObjects;
using Cluster.System.Api;
using System.Security.Principal;
using System.Text;
using NakedObjects.Snapshot;
using NakedObjects.Util;
using System.ComponentModel;
using Cluster.Audit.Api;
using System.Xml.Linq;
namespace Cluster.Audit.Impl
{
    [DisplayName("Auditing")]
    public class AuditService
    {
        #region Injected Services

        public IXmlSnapshotService XmlSnapshotService { set; protected get; }

        public IDomainObjectContainer Container { set; protected get; }

        public IClock Clock { set; protected get; }
        #endregion


        [MemberOrder(10)]
        public IQueryable<AuditedEvent> RecentAuditedEvents()
        {
            return Container.Instances<AuditedEvent>().OrderByDescending(x => x.DateTime);
        }

        [MemberOrder(20)]
        public IQueryable<AuditedEvent> FindAuditedEvents(
            [Optionally] DateTime? fromDate,
            [Optionally] DateTime? toDate,
            string userName)
        {
            var q = RecentAuditedEvents();
            q = FilterForDatesAndUserName(fromDate, toDate, userName, q);
            return q;
        }

        internal static IQueryable<AuditedEvent> FilterForDatesAndUserName(DateTime? fromDate, DateTime? toDate, string userName, IQueryable<AuditedEvent> q)
        {
            if (fromDate != null)
            {
                var d = fromDate.Value.Date;
                q = q.Where(x => x.DateTime >= d);
            }
            if (toDate != null)
            {
                var d = toDate.Value.Date;
                q = q.Where(x => x.DateTime <= d);
            }
            if (userName != null)
            {
                q = q.Where(x => x.UserName.ToUpper().Contains(userName.ToUpper()));
            }
            return q;
        }

        [NakedObjectsIgnore]
        public void ObjectActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters)
        {
            if (queryOnly && !AppSettings.AuditQueryOnlyActions()) return;

            var ae = NewTransientAuditedEvent<ObjectAction>(byPrincipal);
            ae.Object = onObject as IDomainInterface;
            ae.Action = NameUtils.NaturalName(actionName);
            ae.Parameters = ParamsAsString(withParameters);
            Container.Persist(ref ae);
        }

        private static string ParamsAsString(object[] withParameters)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < withParameters.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(withParameters[i] ?? "-");
            }
            string paramString = sb.ToString();
            return paramString;
        }


        private T NewTransientAuditedEvent<T>(IPrincipal byPrincipal) where T : AuditedEvent, new()
        {
            var ae = Container.NewTransientInstance<T>();
            ae.UserName = byPrincipal.Identity.Name;
            ae.DateTime = Clock.Now();
            return ae;
        }

        [NakedObjectsIgnore]
        public void ServiceActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters)
        {
            if (serviceName == "Auditing") return;  //Audit Service actions are not themselves audited
            if (queryOnly && !AppSettings.AuditQueryOnlyActions()) return;

            var ae = NewTransientAuditedEvent<ServiceAction>(byPrincipal);
            (ae as ServiceAction).ServiceName = serviceName;
            ae.Action = NameUtils.NaturalName(actionName);
            ae.Parameters = ParamsAsString(withParameters);
            Container.Persist(ref ae);
        }

        [NakedObjectsIgnore]
        public void ObjectPersisted(IPrincipal byPrincipal, object persistedObject)
        {
            ObjectChanged<ObjectPersisted>(byPrincipal, persistedObject);
        }

        [NakedObjectsIgnore]
        public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject)
        {
            ObjectChanged<ObjectUpdated>(byPrincipal, updatedObject);
        }

        private void ObjectChanged<T>(IPrincipal byPrincipal, object changedObject) where T : ObjectUpdated, new()
        {
            if (!typeof(IDomainInterface).IsAssignableFrom(changedObject.GetType())) return;
            Type objectType =TypeUtils.GetProxiedType(changedObject.GetType());
            if (objectType.Namespace.StartsWith(this.GetType().Namespace)) return;  // Else get stack overlfow!

            var ae = NewTransientAuditedEvent<T>(byPrincipal);
            ae.Object = (IDomainInterface)changedObject;
            var xml = XmlSnapshotService.GenerateSnapshot(changedObject).Xml;
            ae.Snapshot = FormatXML(xml);
            Container.Persist(ref ae);
        }

        private static string FormatXML(string inputXML)
        {
            var output = new StringBuilder();

            if (!string.IsNullOrEmpty(inputXML))
            {
                XElement.Parse(inputXML).Elements().ToList().ForEach(n => output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n"));
            }
            return output.ToString();
        }
    }
}
