using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Audit.Test
{
    public class MockAudited : IDomainInterface
    {
		#region LifeCycle Methods

		public IDomainObjectContainer Container { set; protected get; }
		#endregion

		public virtual int Id { get; set; }
		
        public override string ToString()
        {
			var t = Container.NewTitleBuilder(); // revised for NOF7
            t.Append(Name);
            return t.ToString();
        }

        [Optionally]
        public virtual string Name { get; set; }

        public void DoSomething()
        {

        }
      
    }
}
