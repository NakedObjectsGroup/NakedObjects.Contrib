using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace Cluster.Audit.Impl
{
     [Immutable(WhenTo.OncePersisted)]
    public class ServiceAction : AuditedEvent
    {

         public override string ToString()
         {
             TitleBuilder t = new TitleBuilder();
             t.Append("Action:").Append(Action);
             return t.ToString();
         }

         [MemberOrder(30)]
        public virtual string ServiceName { get; set; }

        [MemberOrder(40)]
        public virtual string Action { get; set; }

        [MemberOrder(50)]
        public virtual string Parameters { get; set; }
      
    }
}
