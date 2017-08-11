using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.Tasks.Api
{

    /// <summary>
    /// A 'role' interface, to be implemented by a service in another cluster that is capable of finding Task Assignees
    /// </summary>
    public interface ITaskAssigneeFinder
    {
        ITaskAssignee CurrentUser();

        ITaskAssignee FindById(int id);

        IQueryable<ITaskAssignee> FindTaskAssignee(string nameMatch);
    }
}
