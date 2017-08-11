using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Audit.Impl
{
    [Immutable(WhenTo.OncePersisted)]
    public abstract class AuditedEvent : IDomainInterface
    {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }
        
        [MemberOrder(10)]
        public virtual DateTime DateTime { get; set; }
  
        [MemberOrder(20)]
        public virtual string UserName { get; set; }
     
    }
}
