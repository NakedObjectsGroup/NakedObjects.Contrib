using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cluster.Users.Api;
using Cluster.Tasks.Api;
using NakedObjects;


namespace Cluster.Tasks.Impl
{
    public class TaskContributedActions
    {

        public void AssignToMe(IQueryable<Task> tasks)
        {
            foreach (Task t in tasks.ToList())
            {
                t.AssignToMe(false);
            }

        }

        [NotContributedAction(typeof(IUser))]
        public void AssignTo(IQueryable<Task> tasks, IUser user)
        {
            foreach (Task t in tasks.ToList())
            {
                t.AssignTo(user);
            }
        }

        public void Cancel(IQueryable<Task> tasks, string reason)
        {
            foreach (Task t in tasks.ToList())
            {
                t.Cancel(reason);
            }
        }
    }
}
