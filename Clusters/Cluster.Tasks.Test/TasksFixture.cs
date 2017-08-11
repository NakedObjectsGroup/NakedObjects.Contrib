using NakedObjects;
using System;
using System.Data.Entity;
using System.IO;
using Cluster.Tasks.Api;
using Cluster.Tasks.Impl;
using Cluster.Users.Api;
using Cluster.Users.Mock;

namespace Cluster.Tasks.Test
{
    public class TasksFixture
    {
                #region Injected Services
        
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

      public  void Install()
        {
            var role = NewRole( TasksRoles.TaskAssignee);
            MockUsersFixture.Richard.Roles.Add(role);
            MockUsersFixture.Robbie.Roles.Add(role);
            MockUsersFixture.Charlie.Roles.Add(role);

            AllTaskTypes();
            Tasks();
        }

             public static TaskType ttypeNewForm;
        public static TaskType typeA;
        public static TaskType typeB;
        public const string reminderTaskName = "Reminder";

        public  void AllTaskTypes()
        {
            ttypeNewForm = NewTaskType( Cluster.Forms.Api.Constants.NewFormSubmission, typeof(Task), false);
            typeA = NewTaskType( "TypeA", typeof(Task));
            typeB = NewTaskType( "TypeB", typeof(Task));
            NewTaskType( reminderTaskName, typeof(Task));
        }

        public  TaskType NewTaskType(string name, Type correspondingClass, bool userCreatable = true, string description = null)
        {
            var tt = Container.NewTransientInstance<TaskType>();
            tt.Name = name;
                tt.CorrespondingClass = correspondingClass.FullName;
                tt.Description = description;
                tt.UserCreatable = userCreatable;
                Container.Persist(ref tt);
            return tt;
        }

        public  void Tasks()
        {
            var today = new DateTime(2000, 1, 1);
            //TODO:  Order in which mock users are persisted is brittle!
            int richard = 1;
            int robert = 2;
            int robbie = 3;
            int charlie = 4;

            NewTask(1, typeA, richard); //1
            NewTask(2, typeA, richard); //2
            NewTask(3, typeA, richard); //3
            NewTask(4, typeA, robert); //4
            NewTask(5, typeB, robert); //5
            NewTask(6, typeB, robert); //6
            NewTask(7, ttypeNewForm, robbie); //7
            NewTask(8, typeB, robbie); //8
            NewTask(9, typeB, charlie, TaskStatusValues.Completed); //9
            NewTask(10, typeA, charlie); //10
            NewTask(11, typeA, charlie); //11
            NewTask(12, typeA, charlie); //12
            NewTask(13, typeA, charlie); //13
            NewTask(14, typeA, charlie); //14
            NewTask(15, typeA, charlie); //15
            NewTask(16, typeA, charlie, TaskStatusValues.Started); //16
            NewTask(17, typeA, charlie, TaskStatusValues.Completed); //17
            NewTask(18, typeA, charlie, TaskStatusValues.Cancelled); //18
            NewTask(19, typeA, charlie, TaskStatusValues.Started); //19
            NewTask(20, typeA, charlie, TaskStatusValues.Started); //20
            NewTask(21, typeA, charlie, TaskStatusValues.Completed); //21
            NewTask(22, typeA, charlie, TaskStatusValues.Cancelled); //22
            NewTask(23, typeA, charlie, TaskStatusValues.Pending); //23
            NewTask(24, typeA, charlie, TaskStatusValues.Started); //24
            NewTask(25, typeA, charlie, TaskStatusValues.Pending); //25
            NewTask(26, typeA, charlie, TaskStatusValues.Pending); //26
            NewTask(27, typeA, charlie, TaskStatusValues.Completed); //27
            NewTask(28, typeA, charlie, TaskStatusValues.Cancelled); //28
            NewTask(29, typeA, charlie, TaskStatusValues.Pending); //29
            NewTask(30, typeA, charlie, TaskStatusValues.Pending); //30
            NewTask(31, typeA, charlie, TaskStatusValues.Pending); //31
            NewTask(32, typeA, charlie, TaskStatusValues.Suspended, new DateTime(2013, 1, 1)); //32
            NewTask(33, typeA, charlie, TaskStatusValues.Suspended, today); //33
            NewTask(34, typeA, charlie, TaskStatusValues.Suspended, today); //34
            NewTask(35, typeB, charlie, TaskStatusValues.Suspended, today); //35

            NewTask(36, typeA, charlie, TaskStatusValues.Suspended, today); //36  
            NewTask(37, typeA, charlie, TaskStatusValues.Suspended, today); //37
            NewTask(38, typeB, charlie, TaskStatusValues.Suspended, today); //38

        }

        public  Task NewTask(
            int testNo, TaskType type,
            int userId,
            TaskStatusValues status = TaskStatusValues.Pending,
            DateTime? suspendedUntil = null)
        {
            var task = Container.NewTransientInstance<Task>();
            task.Type = type;
                task.SuspendedUntil = suspendedUntil;
                task.AssignedToId = userId;
                task.Notes = "Test" + testNo + "\n";
                Container.Persist(ref task);
                task.Status = status;
            return task;
        }

        public  void CreateTaskScheduledProcesses()
        {
            NewCreateTaskScheduledProcess( reminderTaskName, "Create Monthly Reports");
            NewCreateTaskScheduledProcess( reminderTaskName, "Chase overdue debtors");
        }

        public  CreateTaskBatchProcess NewCreateTaskScheduledProcess(
            string taskName,
            string notes)
        {
            var ctsp = Container.NewTransientInstance<CreateTaskBatchProcess>();
            ctsp.TaskName = taskName;
                ctsp.NotesToAddToTask = notes;
                Container.Persist(ref ctsp);
            return ctsp;
        }


        public MockRole NewRole(string roleName)
        {
            MockRole role = Container.NewTransientInstance<MockRole>();
            role.Name = roleName;
            Container.Persist(ref role);
            return role;
        }
    }
}

