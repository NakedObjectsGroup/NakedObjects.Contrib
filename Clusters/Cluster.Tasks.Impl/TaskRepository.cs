using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Cluster.Users.Api;
using Cluster.Tasks.Api;
using NakedObjects;
using NakedObjects.Util;

namespace Cluster.Tasks.Impl
{
    [DisplayName("Tasks")]
    public class TaskRepository : ITaskService
    {
        #region
        public IDomainObjectContainer Container { set; protected get; }

        public IUserService UserFinder { set; protected get; }

        #endregion

        #region FindById
        public Task FindById(int id)
        {
            return Container.Instances<Task>().Where(x => x.Id == id).FirstOrDefault();
        }
        #endregion

        #region FindTasks
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "AssignedTo", "Status","Type", "Due")]
        public IQueryable<Task> FindTasks([Optionally] IUser assignedTo, TaskStatusValues status, [Optionally] TaskType type)
      {
          var q =  Container.Instances<Task>().Where(x =>
              (status == TaskStatusValues.Any || x.Status == status));
           if (type != null) {
            q = q.Where(x => x.Type.Id == type.Id);
           }
           if (assignedTo != null)
           {
               q = q.Where(x => x.AssignedToId == assignedTo.Id);
           }
            return q.OrderBy(x => x.Due);
      }

        public TaskStatusValues Default1FindTasks()
        {
            return TaskStatusValues.Any;
        }
        #endregion

        #region MyAssignedTasks
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "Type", "Due", "Notes")]
        public IQueryable<Task> MyAssignedTasks()
        {
            var me = UserFinder.CurrentUser();
            return FindTasks(me, TaskStatusValues.Pending, null);
        }
        #endregion

        #region CreateNewTask
        public ITask CreateNewTask(TaskType type, [Optionally] bool assignToMe)
        {
            return CreateTask(type, assignToMe);
        }

        private Task CreateTask(TaskType type, bool assignToMe)
        {
           IUser assignedTo = null;
            if (assignToMe)
            {
                assignedTo = UserFinder.CurrentUser();
            }
            return CreateTask(type, assignedTo);
        }

        private Task CreateTask(TaskType type, IUser assignedTo)
        {
            Type t = TypeUtils.GetType(type.CorrespondingClass);
            Task task = (Task)Container.NewTransientInstance(t);
            task.Type = type;
            task.AssignedTo = assignedTo;
            return task;
        }

        public IList<TaskType> Choices0CreateNewTask()
        {
            return Container.Instances<TaskType>().Where(x => x.UserCreatable).ToList();
        }

        [NakedObjectsIgnore]
        public ITask CreateNewTaskAssignedToMe(string taskTypeName, string notes = null, params ITaskContext[] contexts)
        {
            var task = CreateTask(TypeForName(taskTypeName), true);
            task.Notes = notes;
            //TODO:  Add contexts
            //TODO:  Factor out common logic from multple create methods; reduce methods if poss.
            Container.Persist(ref task);
            return task;
        }

        [NakedObjectsIgnore]
        public ITask CreateNewTask(string taskTypeName, IUser assignedTo, string notes = null, params ITaskContext[] contexts)
        {
            var task = CreateTask(TypeForName(taskTypeName), assignedTo);
            task.Notes = notes;
            //TODO:  Add contexts

            Container.Persist(ref task);
            return task;
        }

        #endregion

        #region TaskTypes
        private TaskType TypeForName(string taskTypeName)
        {
            return Container.Instances<TaskType>().Single(x => x.Name == taskTypeName);
        }
        #endregion
    }
}
