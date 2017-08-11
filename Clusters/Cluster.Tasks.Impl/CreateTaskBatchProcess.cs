using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Batch.Api;
using Cluster.Tasks.Api;
using Cluster.Users.Api;
using NakedObjects;

namespace Cluster.Tasks.Impl
{
    public class CreateTaskBatchProcess : IPersistentBatchProcess
    {
        #region Injected Services
        public ITaskService TaskService { set; protected get; }

        public IUserService UserService { set; protected get; }

        #endregion

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }
       
        public virtual string TaskName { get; set; }
              
        public virtual string NotesToAddToTask { get; set; }
       
        public virtual string UserNameToAssignTaskTo { get; set; }
     
      
        public string Invoke()
        {
            IUser user = UserService.FindUserByUserName(UserNameToAssignTaskTo);
            ITask task = TaskService.CreateNewTask(TaskName, user, NotesToAddToTask);
            return "Task Created OK";
        }
    }
}
