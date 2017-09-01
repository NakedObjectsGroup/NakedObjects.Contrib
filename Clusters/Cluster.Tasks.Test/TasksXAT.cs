using System;
using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Tasks.Impl;
using Cluster.Tasks.Api;
using Cluster.Users.Mock;
using Cluster.System.Mock;
using Helpers;
using NakedObjects.Services;

namespace Cluster.Tasks.Test
{
    [TestClass]
    public class TasksXAT : ClusterXAT<TasksTestDbContext, TasksFixture>
    {
        #region Run configuration

        private readonly System.Api.IClock _clock = new FixedClock(new DateTime(2000, 1, 1));

        //Set up the properties in this region exactly the same way as in your Run class
        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller
                    (new MockUserService(),
                    new TaskRepository(),
                    new SimpleRepository<UnsuspendTasksBatchProcess>(),
                    new TestRepository(),
                    _clock
                    );
            }
        }

        protected override IFixturesInstaller Fixtures
        {
            get
            {
                return new FixturesInstaller(
                    new MockUsersFixture(),
                    new TasksFixture());
            }
        }

        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        //[ClassInitialize()]
        //public static void ClassInit()
        //{
        //    DatabaseUtils.SnapshotDatabase("DataAccess.MyDbContext", "xat-tasks");
        //}

        //[ClassCleanup()]
        //public void ClassCleanup()
        //{
        //    DatabaseUtils.RestoreDatabase("DataAccess.MyDbContext", "xat-tasks");
        //}

        #endregion

        #region Task Types
		[TestMethod, TestCategory("TasksXAT")]
        public void TaskType()
        {
            var t1 = GetBoundedInstance<TaskType>("TypeA");
            t1.AssertIsPersistent().AssertIsImmutable().AssertTitleEquals("TypeA");
        }
        #endregion

        #region Creating Tasks
		[TestMethod, TestCategory("TasksXAT")]
        public void CreateNewTask()
        {
            var t1 = GetBoundedInstance<TaskType>("TypeA");
            var create = GetTestService("Tasks").GetAction("Create New Task");
            var task = create.InvokeReturnObject(t1, false);
            task.AssertIsTransient().AssertIsType(typeof(Task));
            task.GetPropertyByName("Type").AssertIsVisible().AssertTitleIsEqual("TypeA").AssertIsUnmodifiable();
            task.GetPropertyByName("Id").AssertIsInvisible();
            var status = task.GetPropertyByName("Status").AssertIsVisible().AssertIsUnmodifiable().AssertValueIsEqual("Unsaved");
            task.GetPropertyByName("Due").AssertIsVisible().AssertIsModifiable().AssertIsEmpty().AssertIsOptional();
            var assignedTo = task.GetPropertyByName("Assigned To").AssertIsVisible().AssertIsModifiable().AssertIsEmpty().AssertIsMandatory();
            task.AssertCannotBeSaved();

            var richard = GetUser("Richard");
            assignedTo.SetObject(richard);
            assignedTo.AssertTitleIsEqual("Richard");
            task.AssertCanBeSaved();
            task.Save().AssertIsPersistent();
            status.AssertValueIsEqual("Pending");

            AssertLastHistoryEntryHasValues(task, "Pending", "Richard", "Test");
        }

        private void AssertLastHistoryEntryHasValues(ITestObject task, string status, string assignedTo, string changedBy, DateTime? dt = null)
        {
                 var hist = task.GetPropertyByName("History").ContentAsCollection;
            var last = hist.Last();
            last.GetPropertyByName("Status").AssertTitleIsEqual(status);
            last.GetPropertyByName("Assigned To").AssertTitleIsEqual(assignedTo);
            last.GetPropertyByName("Change Made By").AssertTitleIsEqual(changedBy);
            var dateTime = last.GetPropertyByName("Date Time");
            if (dt == null)
            {
                dateTime.AssertIsNotEmpty();
            }
            else
            {
                dateTime.AssertTitleIsEqual(dt.ToString());
            }
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void CreateNewTaskAssignedToMe()
        {
            SetUser("Robbie");
            var t1 = GetBoundedInstance<TaskType>("TypeA");
            var create = GetTestService("Tasks").GetAction("Create New Task");
            var task = create.InvokeReturnObject(t1, true);
            var assignedTo = task.GetPropertyByName("Assigned To").AssertIsVisible().AssertIsModifiable().AssertTitleIsEqual("Robbie");

            //task.GetPropertyByName("Notes").SetValue("xxx");
            //task.GetPropertyByName("Due").SetValue("01/01/2001 00:00:00");
            
            
            task.AssertCanBeSaved();
            task.Save().AssertIsPersistent();

            AssertLastHistoryEntryHasValues(task, "Pending", "Robbie", "Robbie");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void OnlyUserCreatableTypesShowninDropDown()
        {
            var t1 = GetBoundedInstance<TaskType>("TypeB");
            t1.GetPropertyByName("User Creatable").AssertValueIsEqual("True");
            var nfs = GetBoundedInstance<TaskType>("New Form Submission");
            nfs.GetPropertyByName("User Creatable").AssertValueIsEqual("False");
            var create = GetTestService("Tasks").GetAction("Create New Task");
            var types = create.Parameters.ElementAt(0).GetChoices();
            Assert.IsTrue(types.Contains(t1));
            Assert.IsFalse(types.Contains(nfs));

        }
        #endregion

        #region Finding Tasks
		[TestMethod, TestCategory("TasksXAT")]
        public void FindById()
        {
            var task = GetTestService("Tasks").GetAction("Find By Id").InvokeReturnObject(1);
            task.AssertTitleEquals("Task # 1");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void FindTasks()
        {
            var find = GetTestService("Tasks").GetAction("Find Tasks");
            var tasks = find.InvokeReturnCollection(null, TaskStatusValues.Any, null);
            tasks.AssertIsNotEmpty();

            tasks = find.InvokeReturnCollection(null, TaskStatusValues.Completed, null);
            var matches = tasks.Count(x => x.GetPropertyByName("Status").Title == "Completed");
            Assert.AreEqual(tasks.Count(), matches);


            var typeA = GetBoundedInstance<TaskType>("TypeA");
            tasks = find.InvokeReturnCollection(null, TaskStatusValues.Any, typeA);
            matches = tasks.Count(x => x.GetPropertyByName("Type").Title == "TypeA");
            Assert.AreEqual(tasks.Count(), matches);


            var richard = GetUser("Richard");
            tasks = find.InvokeReturnCollection(richard, TaskStatusValues.Any, null);
            var first = tasks.ElementAt(0);
            first.GetPropertyByName("Assigned To").AssertTitleIsEqual("Richard");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void MyAssignedTasks1()
        {
            SetUser("Robert");
            var find = GetTestService("Tasks").GetAction("My Assigned Tasks");
            var tasks = find.InvokeReturnCollection();
            tasks.AssertIsNotEmpty();
            var assignedToMe = tasks.Count(x => (x.GetPropertyByName("Assigned To").Title == "Robert")
                && (x.GetPropertyByName("Status").Title == "Pending"));
            Assert.AreEqual(tasks.Count(), assignedToMe);
        }
		[TestMethod, TestCategory("TasksXAT")]
        public void MyAssignedTasks2()
        {
            SetUser("Richard");
            var find = GetTestService("Tasks").GetAction("My Assigned Tasks");
            var tasks = find.InvokeReturnCollection();
            var assignedToMe = tasks.Count(x => (x.GetPropertyByName("Assigned To").Title == "Richard")
                && (x.GetPropertyByName("Status").Title == "Pending"));
            Assert.AreEqual(tasks.Count(), assignedToMe);
        }
        #endregion

        #region Assignment
		[TestMethod, TestCategory("TasksXAT")]
        public void Assign()
        {
            var task = GetTaskWithTestNumber(10);
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var richard = GetUser("Richard");
            task.GetAction("Assign To").InvokeReturnObject(richard);
            assignedTo.AssertObjectIsEqual(richard);
            status.AssertTitleIsEqual("Pending");
        }



		[TestMethod, TestCategory("TasksXAT")]
        public void AttemptReassignToSameUser()
        {
            var task = GetTaskWithTestNumber(11);
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            var charlie = assignedTo.ContentAsObject;
            task.GetAction("Assign To").AssertIsInvalidWithParms(charlie);
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void AssignToMe()
        {
            SetUser("Richard");
            var task = GetTaskWithTestNumber(12);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Assign To Me").InvokeReturnObject(false);
            assignedTo.AssertTitleIsEqual("Richard");
            status.AssertTitleIsEqual("Pending");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void AssignActionsInvisibleIfTaskIsCompletedOrCancelled()
        {
            var task = GetTaskWithTestNumber(16);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Started");
            task.GetAction("Assign To").AssertIsVisible();
            task.GetAction("Assign To Me").AssertIsVisible();

            task = GetTaskWithTestNumber(17);
            status = task.GetPropertyByName("Status").AssertTitleIsEqual("Completed");
            task.GetAction("Assign To").AssertIsInvisible();
            task.GetAction("Assign To Me").AssertIsInvisible();

            task = GetTaskWithTestNumber(18);
            status = task.GetPropertyByName("Status").AssertTitleIsEqual("Cancelled");
            task.GetAction("Assign To").AssertIsInvisible();
            task.GetAction("Assign To Me").AssertIsInvisible();
        }
        #endregion

        #region Start

		[TestMethod, TestCategory("TasksXAT")]
        public void StartTaskAlreadyAssignedToMe()
        {
            SetUser("Charlie");
            var task = GetTaskWithTestNumber(14);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Start").InvokeReturnObject();
            status.AssertTitleIsEqual("Started");

            AssertLastHistoryEntryHasValues(task, "Started", "Charlie", "Charlie");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void CantStartTaskIfNotAssignedToMe()
        {
            SetUser("Richard");
            var task = GetTaskWithTestNumber(15); //re-using as no change made
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Start").AssertIsDisabled();
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void AssignToMeAndStart()
        {
            SetUser("Richard");
            var task = GetTaskWithTestNumber(13);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Assign To Me").InvokeReturnObject(true);
            assignedTo.AssertTitleIsEqual("Richard");
            status.AssertTitleIsEqual("Started");
            AssertLastHistoryEntryHasValues(task, "Started", "Richard", "Richard");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void StartActionHiddenIfAlreadyStartedOrCompletedOCancelled()
        {
            var task = GetTaskWithTestNumber(16);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Started");
            task.GetAction("Start").AssertIsInvisible();

            task = GetTaskWithTestNumber(17);
            status = task.GetPropertyByName("Status").AssertTitleIsEqual("Completed");
            task.GetAction("Start").AssertIsInvisible();

            task = GetTaskWithTestNumber(18);
            status = task.GetPropertyByName("Status").AssertTitleIsEqual("Cancelled");
            task.GetAction("Start").AssertIsInvisible();

        }
        #endregion

        #region Complete
		[TestMethod, TestCategory("TasksXAT")]
        public void CompleteTaskYouveAlreadyStarted()
        {
            SetUser("Charlie");
            var task = GetTaskWithTestNumber(19);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Started");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Complete").AssertIsVisible().AssertIsEnabled().InvokeReturnObject("x");
            status.AssertTitleIsEqual("Completed");

            AssertLastHistoryEntryHasValues(task, "Completed", "Charlie", "Charlie");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void CantCompleteIfNotAssignedToYou()
        {
            SetUser("Richard");
            var task = GetTaskWithTestNumber(20);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Started");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Complete").AssertIsVisible().AssertIsDisabled();
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void CompleteActionHiddenIfAlreadyCompletedOrCancelled()
        {
            var task = GetTaskWithTestNumber(21);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Completed");
            task.GetAction("Complete").AssertIsInvisible();

            task = GetTaskWithTestNumber(22);
            status = task.GetPropertyByName("Status").AssertTitleIsEqual("Cancelled");
            task.GetAction("Complete").AssertIsInvisible();

        }

		[TestMethod, TestCategory("TasksXAT")]
        public void CantCompleteTaskIfNotStarted()
        {
            SetUser("Charlie");
            var task = GetTaskWithTestNumber(23);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Complete").AssertIsDisabled();
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void CompleteTaskWithComment()
        {
            SetUser("Charlie");
            var task = GetTaskWithTestNumber(24);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Started");
            var assignedTo = task.GetPropertyByName("Assigned To").AssertTitleIsEqual("Charlie");
            task.GetAction("Complete").AssertIsVisible().AssertIsEnabled().InvokeReturnObject("Foo");
            status.AssertTitleIsEqual("Completed");

            AssertLastHistoryEntryHasValues(task, "Completed", "Charlie", "Charlie");
        }
        #endregion

        #region Cancel
		[TestMethod, TestCategory("TasksXAT")]
        public void CancelTask()
        {
            SetUser("Charlie");
            var task = GetTaskWithTestNumber(25);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var cancel = task.GetAction("Cancel").AssertIsVisible();
            cancel.Parameters.ElementAt(0).AssertIsNamed("Reason").AssertIsMandatory();
                cancel.InvokeReturnObject("my reason");
            status.AssertTitleIsEqual("Cancelled");

            AssertLastHistoryEntryHasValues(task, "Cancelled", "Charlie", "Charlie");
        }


		[TestMethod, TestCategory("TasksXAT")]
        public void CancelActionHiddenIfAlreadyCompleteOrCancelled()
        {
            var task = GetTaskWithTestNumber(27);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Completed");
            task.GetAction("Cancel").AssertIsInvisible();

            task = GetTaskWithTestNumber(28);
            status = task.GetPropertyByName("Status").AssertTitleIsEqual("Cancelled");
            task.GetAction("Cancel").AssertIsInvisible();
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void CantCancelIfStartedUnlessAssignedToYou()
        {
        }
        #endregion

        #region Suspension

		[TestMethod, TestCategory("TasksXAT")]
        public void SuspendTaskForDays()
        {
            var task = GetTaskWithTestNumber(29);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var suspend = task.GetAction("Suspend").AssertIsVisible();
            var unsuspend = task.GetAction("Unsuspend").AssertIsInvisible();
            var choices = suspend.Parameters.ElementAt(0).AssertIsNamed("Days").AssertIsOptional().GetChoices();
            var until = task.GetPropertyByName("Suspended Until").AssertIsInvisible();
            Assert.AreEqual(4, choices.Count());
            suspend.Parameters.ElementAt(1).AssertIsNamed("Until").AssertIsOptional();
            suspend.Parameters.ElementAt(2).AssertIsNamed("Comment").AssertIsOptional();
            suspend.InvokeReturnObject(7, null, "foo");
            status.AssertTitleIsEqual("Suspended");
            unsuspend.AssertIsVisible();

			//until.AssertIsVisible().AssertIsNotEmpty().AssertValueIsEqual("08/01/2000 00:00:00");
			var expectedText = $"{UtcAndToString(new DateTime(2000, 1, 8, 0, 0, 0, DateTimeKind.Utc))}";
			until.AssertIsVisible().AssertIsNotEmpty().AssertValueIsEqual(expectedText);

			var notes = task.GetPropertyByName("Notes").Title;
            StringAssert.EndsWith(notes, "Suspended: foo");
            AssertLastHistoryEntryHasValues(task, "Suspended", "Charlie", "Test");
        }
        
		[TestMethod, TestCategory("TasksXAT")]
        public void SuspendTaskToSpecificDate()
        {
            var task = GetTaskWithTestNumber(30);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
            var suspend = task.GetAction("Suspend");
            var until = task.GetPropertyByName("Suspended Until").AssertIsInvisible();
            var todayPlus10 = new DateTime(2000, 1, 11);
            suspend.InvokeReturnObject(null, todayPlus10, null);
            status.AssertTitleIsEqual("Suspended");
            until.AssertIsVisible().AssertIsNotEmpty().AssertValueIsEqual(todayPlus10.Date.ToString()); 
            AssertLastHistoryEntryHasValues(task, "Suspended", "Charlie", "Test");
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void AttemptWithInvalidDates()
        {
            var task = GetTaskWithTestNumber(31);
              var suspend = task.GetAction("Suspend");
              var todayPlus30 = _clock.Today().AddDays(30);
              suspend.AssertIsValidWithParms(null, todayPlus30, null);
              var todayPlus31 = _clock.Today().AddDays(31);
            suspend.AssertIsInvalidWithParms(null, todayPlus31, null);
            var todayMinus1 = _clock.Today().AddDays(-1);
            suspend.AssertIsInvalidWithParms(null, todayMinus1, null);
            var today = _clock.Today();
            suspend.AssertIsInvalidWithParms(null, today, null);
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void MustSpecifyEitherDaysOrUntil()
        {
            var task = GetTaskWithTestNumber(31); //OK to re-use
            var suspend = task.GetAction("Suspend");
            var todayPlus30 = _clock.Today().AddDays(30);
            suspend.AssertIsValidWithParms(null, todayPlus30, null);
            suspend.AssertIsValidWithParms(14, null, null);

            suspend.AssertIsInvalidWithParms(14, todayPlus30, null);
            suspend.AssertIsInvalidWithParms(null, null, null);
        }

		[TestMethod, TestCategory("TasksXAT")]
        public void Unsuspend()
        {
            var task = GetTaskWithTestNumber(32);
            var status = task.GetPropertyByName("Status").AssertTitleIsEqual("Suspended");
            var until = task.GetPropertyByName("Suspended Until").AssertIsVisible().AssertIsNotEmpty();
 
            var unsuspend = task.GetAction("Unsuspend").AssertIsVisible();
            var suspend = task.GetAction("Suspend").AssertIsInvisible();

            unsuspend.Parameters.ElementAt(0).AssertIsNamed("Comment").AssertIsOptional();

            unsuspend.InvokeReturnObject("foo");
            status.AssertTitleIsEqual("Pending");
            until.AssertIsInvisible();

            var notes = task.GetPropertyByName("Notes").Title;
            StringAssert.EndsWith(notes, "Unsuspended: foo");
            AssertLastHistoryEntryHasValues(task, "Pending", "Charlie", "Test");

        }

        #endregion

        #region Task Service 

		[TestMethod, TestCategory("TasksXAT")]
        public void UnsuspendTasksForToday()
        {
            string today = _clock.Today().Date.ToString("d");
            var find = GetTestService("Tasks").GetAction("Find Tasks");
            var tasks = find.InvokeReturnCollection(null, TaskStatusValues.Suspended, null);
            int total = tasks.Count();
            var toProcess = tasks.Where(x => x.GetPropertyByName("Suspended Until").Title == today);
            int remaining = total - toProcess.Count();
            Assert.AreEqual(6, toProcess.Count());
            var first = toProcess.ElementAt(0);

            var process = GetTestService("Unsuspend Tasks Batch Processes").GetAction("New Instance").InvokeReturnObject();
            process.GetAction("Invoke").Invoke();

             tasks = find.InvokeReturnCollection(null, TaskStatusValues.Suspended, null);
             tasks.AssertCountIs(remaining);

             first.GetPropertyByName("Status").AssertTitleIsEqual("Pending");
             first.GetPropertyByName("Suspended Until").AssertIsInvisible();
        }

        #endregion

        #region Helpers

        protected ITestObject GetUser(string name)
        {
            return GetTestService("Users").GetAction("Find User By User Name").InvokeReturnObject(name).AssertTitleEquals(name);
        }

        protected ITestObject GetTaskWithTestNumber(int testTaskNo)
        {
            return GetTestService("Test Repository").GetAction("Find Test Task").InvokeReturnObject(testTaskNo);
           
        }

        #endregion
    }
}