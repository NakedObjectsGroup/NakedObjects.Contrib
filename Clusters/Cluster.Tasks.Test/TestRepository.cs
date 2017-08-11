using System.Linq;
using System.ComponentModel;
using Cluster.Tasks.Impl;
using NakedObjects;
namespace Cluster.Tasks.Test
{

    public class TestRepository
    {

        public IDomainObjectContainer Container { set; protected get; }

       
      #region FindTestTask
        public Task FindTestTask(int testNo)
      {
          string note = "Test" + testNo;
      var query = from obj in Container.Instances<Task>()
		              where obj.Notes.StartsWith(note)
		              select obj;
                  
      return query.FirstOrDefault();
	  //(If inheriting from AbsractFactoryAndRepository can use:) return SingleObjectWarnIfNoMatch(query);
      }
      #endregion
      

    }
}
