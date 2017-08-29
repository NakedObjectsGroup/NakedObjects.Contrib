using NakedObjects;

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
			var t = Container.NewTitleBuilder();
			t.Append("Update:").Append(this.Object.ToString());
            return t.ToString();
        }

        [MemberOrder(40), MultiLine(NumberOfLines= 5)]
        public virtual string Snapshot { get; set; }
    }
}
