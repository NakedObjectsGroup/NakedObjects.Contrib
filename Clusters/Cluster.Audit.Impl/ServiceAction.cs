using NakedObjects;

namespace Cluster.Audit.Impl
{
    [Immutable(WhenTo.OncePersisted)]
    public class ServiceAction : AuditedEvent
    {
		public IDomainObjectContainer Container { set; protected get; }

		public override string ToString()
        {
			var t = Container.NewTitleBuilder();
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
