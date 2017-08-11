using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cluster.System.Api;
using NakedObjects;
using NakedObjects.Services;

namespace Cluster.Audit.Impl
{
     [Immutable(WhenTo.OncePersisted)]
    public class ObjectUpdated : ObjectAuditedEvent
    {
        #region Injected Services
        #endregion

        #region LifeCycle methods
#endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("Update:").Append(this.Object.ToString());
            return t.ToString();
        }

        [MemberOrder(40), MultiLine(NumberOfLines= 5)]
        public virtual string Snapshot { get; set; }
    }
}
