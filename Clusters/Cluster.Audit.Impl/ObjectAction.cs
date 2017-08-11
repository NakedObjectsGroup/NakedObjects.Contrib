using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Cluster.System.Api;
using NakedObjects;
using NakedObjects.Services;

namespace Cluster.Audit.Impl
{
     [Immutable(WhenTo.OncePersisted)]
    public class ObjectAction : ObjectAuditedEvent
    {
        #region Injected Services
        #endregion

        #region LifeCycle methods
        #endregion

        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("Action:").Append(Action);
            return t.ToString();
        }
      

        [MemberOrder(40)]
        public virtual string Action { get; set; }

        [MemberOrder(50)]
        public virtual string Parameters { get; set; }
               
    }
}
