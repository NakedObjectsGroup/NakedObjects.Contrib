using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cluster.Names.Api;
using NakedObjects;

namespace Cluster.Names.Test
{
    public class TestIndividual : IIndividualWithClusterManagedName
    {
        #region Injected Services
        public INameService NameService { set; protected get; }

        #endregion

        public virtual int Id { get; set; }
		
        #region Name Property of type IName ('Result' interface)

        [Hidden(WhenTo.Always)]
        public virtual int NameId { get; set; }


        private IName _name;

        [NotPersisted(), NotMapped]
        public IName Name
        {
            get
            {
                if (_name == null && NameId > 0)
                {
                    _name = NameService.FindById(NameId);
                }
                return _name;
            }
        }
        #endregion
    }
	
    public class TestIndividualFinder
    {
        public IDomainObjectContainer Container { set; protected get; }

        public INameService NameService { set; protected get; }

        public IQueryable<TestIndividual> FindByName(string match)
        {
            return NameService.FindByName<TestIndividual>(match);
        }

        public TestIndividual NewIndividual()
        {
            return Container.NewTransientInstance<TestIndividual>();
        }
    }
}
