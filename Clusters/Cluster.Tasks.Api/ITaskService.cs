using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cluster.Users.Api;
using NakedObjects;

namespace Cluster.Tasks.Api
{
    public interface ITaskService
    {
        ITask CreateNewTask(string taskTypeName, IUser assignedTo, string notes = null, params ITaskContext[] contexts);

        ITask CreateNewTaskAssignedToMe(string taskTypeName, string notes = null, params ITaskContext[] contexts);
    }
}
