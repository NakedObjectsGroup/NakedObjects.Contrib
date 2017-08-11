using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;
using NakedObjects.Services;

namespace Cluster.Audit.Impl
{
    [DisplayName("Auditing")]
    public class AuditContributedActions
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }
        #endregion

        [MemberOrder(10)]
        public AuditedEvent LastAuditedEvent(IDomainInterface onObject)
        {
            return RecentAuditedEvents(onObject).FirstOrDefault();
        }

         [MemberOrder(20)]
        public IQueryable<AuditedEvent> RecentAuditedEvents(IDomainInterface onObject) {
            return PolymorphicNavigator.FindOwners<ObjectAuditedEventTargetObjectLink, IDomainInterface, ObjectAuditedEvent>(onObject).OrderByDescending(x => x.DateTime);
        }

         [MemberOrder(30)]
        public IQueryable<AuditedEvent> FindAuditedEvents(
            IDomainInterface onObject, 
            [Optionally] DateTime? fromDate,
            [Optionally] DateTime? toDate,
            string userName) {

                var q = RecentAuditedEvents(onObject);
                return AuditService.FilterForDatesAndUserName(fromDate, toDate, userName, q);
        }
    }
}
