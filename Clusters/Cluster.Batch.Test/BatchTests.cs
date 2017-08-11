using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Batch.Impl;
using System.Data.Entity;
using Cluster.System.Mock;
using System;
using Cluster.Users.Mock;
using Helpers;

namespace Cluster.Batch.Test
{

    [TestClass()]
    public class BatchTests : ClusterXAT<BatchTestDbContext, BatchFixture>
    {

       #region Run configuration
        //Set up the properties in this region exactly the same way as in your Run class
       protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new BatchRepository(),
                    new FixedClock(new DateTime(2000, 1, 1)),
                    new MockUserService());
            }
        }

       protected override IFixturesInstaller Fixtures
       {
           get
           {
               return new FixturesInstaller(
                   new BatchFixture(),
                   new MockUsersFixture());
           }
       }
        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion

        [TestMethod()]
        public void CreateNewProcess()
        {
            var proc = GetTestService("Batch").GetAction("Create New Process Definition").InvokeReturnObject();
            proc.AssertIsTransient().AssertIsType(typeof(BatchProcessDefinition));

            var name = proc.GetPropertyByName("Name").AssertIsEmpty().AssertIsMandatory();

            var desc = proc.GetPropertyByName("Description").AssertIsEmpty().AssertIsOptional();

            var status = proc.GetPropertyByName("Status").AssertTitleIsEqual("Active").AssertIsMandatory().AssertIsUnmodifiable();

            var className = proc.GetPropertyByName("Class To Invoke").AssertIsEmpty().AssertIsMandatory();

            var instance = proc.GetPropertyByName("Process Instance Id").AssertIsOptional().AssertIsEmpty();

            var priority = proc.GetPropertyByName("Priority").AssertIsMandatory().AssertValueIsEqual("1");

            var attempts = proc.GetPropertyByName("Number Of Attempts Each Run").AssertIsMandatory().AssertValueIsEqual("1");

            var firstRun = proc.GetPropertyByName("First Run").AssertIsMandatory().AssertValueIsEqual("01/01/2000 00:00:00");

            var frequency = proc.GetPropertyByName("Frequency").AssertIsMandatory().AssertValueIsEqual("Manual Runs Only");

            var nextRun = proc.GetPropertyByName("Next Run").AssertIsUnmodifiable().AssertIsEmpty();

            var lastRun = proc.GetPropertyByName("Last Run").AssertIsOptional().AssertIsEmpty();
        }

        [TestMethod()]
        public void ValidateClassToInvokeAndInstance()
        {
            var proc = GetTestService("Batch").GetAction("Create New Process Definition").InvokeReturnObject();

            var name = proc.GetPropertyByName("Name").SetValue("Foo");

            var className = proc.GetPropertyByName("Class To Invoke");

            var instance = proc.GetPropertyByName("Process Instance Id").AssertIsOptional().AssertIsEmpty();

            className.SetValue("Cluster.Batch.Test.MockSP");
            proc.AssertCanBeSaved();

            instance.SetValue("1");
            proc.AssertCannotBeSaved();

            className.SetValue("Cluster.Batch.Test.MockPersistentSP");
            proc.AssertCanBeSaved();

            instance.ClearValue();
            proc.AssertCannotBeSaved();

            className.SetValue("Cluster.Batch.Test.MockNotAnSP");
            proc.AssertCannotBeSaved();

            instance.SetValue("1");
            proc.AssertCannotBeSaved();

        }

        [TestMethod()]
        public void RetrievePersistentProcessInstance()
        {
            var find = GetTestService("Batch").GetAction("Find Process Definitions");
            var proc = find.InvokeReturnCollection("Process5", null).ElementAt(0);
            proc.AssertTitleEquals("Process5");

            var mock = proc.GetAction("Retrieve Persistent Process Object").InvokeReturnObject();
            mock.AssertIsPersistent().AssertIsType(typeof(MockPersistentSP));
            mock.GetPropertyByName("Id").AssertValueIsEqual("1");
        }

        [TestMethod()]
        public void RetrievePersistentProcessInstanceNotVisibleWhenProcessNotPersistent()
        {
            var find = GetTestService("Batch").GetAction("Find Process Definitions");
            var proc = find.InvokeReturnCollection("Process4", null).ElementAt(0);
            proc.AssertTitleEquals("Process4");

            proc.GetAction("Retrieve Persistent Process Object").AssertIsInvisible() ;
        }
        [TestMethod]
        public void ValidatePriority()
        {
            var proc = GetTestService("Batch").GetAction("Create New Process Definition").InvokeReturnObject();

            var priority = proc.GetPropertyByName("Priority").AssertIsMandatory().AssertValueIsEqual("1");

            priority.AssertFieldEntryIsValid("999");
            priority.AssertFieldEntryInvalid("1000");
            priority.AssertFieldEntryInvalid("0");
            priority.AssertFieldEntryInvalid("-1");
            priority.AssertFieldEntryInvalid("1.1");
        }

        [TestMethod]
        public void ValidateNumberOfAttempts()
        {
            var proc = GetTestService("Batch").GetAction("Create New Process Definition").InvokeReturnObject();
            var name = proc.GetPropertyByName("Name").SetValue("Foo");


            var priority = proc.GetPropertyByName("Number Of Attempts Each Run").AssertIsMandatory().AssertValueIsEqual("1");

            priority.AssertFieldEntryIsValid("1");
            priority.AssertFieldEntryIsValid("9");
            priority.AssertFieldEntryInvalid("10");
            priority.AssertFieldEntryInvalid("0");
            priority.AssertFieldEntryInvalid("-1");
            priority.AssertFieldEntryInvalid("1.1");

        }

        //Minimal test that validation is being picked up as there is separate unit test for the validate method
        [TestMethod()]
        public void ValidateDates()
        {
            var proc = GetTestService("Batch").GetAction("Create New Process Definition").InvokeReturnObject();
            proc.GetPropertyByName("Name").SetValue("Foo");
            proc.GetPropertyByName("Class To Invoke").SetValue("Cluster.Batch.Test.MockSP");

            var firstRun = proc.GetPropertyByName("First Run").AssertIsMandatory().AssertValueIsEqual("01/01/2000 00:00:00");

            var lastRun = proc.GetPropertyByName("Last Run").AssertIsOptional().AssertIsEmpty();

            firstRun.SetValue("03/01/2000");
            lastRun.SetValue("02/01/2000");

            proc.AssertCannotBeSaved();

            firstRun.SetValue("01/01/2000");

            proc.AssertCanBeSaved();
        }


        [TestMethod()]
        public void SaveNewProcessAndTestNextRunDate()
        {
            var proc = GetTestService("Batch").GetAction("Create New Process Definition").InvokeReturnObject();

            proc.GetPropertyByName("Name").SetValue("Foo");

            var className = proc.GetPropertyByName("Class To Invoke").SetValue("Cluster.Batch.Test.MockSP");


            var firstRun = proc.GetPropertyByName("First Run").SetValue("01/02/2000 00:00:00");

            var frequency = proc.GetPropertyByName("Frequency").SetValue(Frequency.MonthlyOn1stOfMonth.ToString());

            proc.Save();
            proc.AssertIsPersistent();

            proc.GetPropertyByName("Next Run").AssertValueIsEqual("01/02/2000 00:00:00");
        }

        [TestMethod()]
        public void StatusChanges()
        {
            var proc = GetTestService("Batch").GetAction("Find Process Definitions").InvokeReturnCollection("Process1", null).ElementAt(0);
            proc.AssertTitleEquals("Process1");

            var status = proc.GetPropertyByName("Status").AssertIsMandatory().AssertIsUnmodifiable();
            var firstRun = proc.GetPropertyByName("First Run").AssertIsNotEmpty();
            var frequency = proc.GetPropertyByName("Frequency").AssertIsNotEmpty();
            var nextRun = proc.GetPropertyByName("Next Run").AssertIsNotEmpty();
            var lastRun = proc.GetPropertyByName("Last Run").AssertIsNotEmpty();

            var suspend = proc.GetAction("Suspend");
            var resume = proc.GetAction("Resume");
            var archive = proc.GetAction("Archive");

            status.AssertTitleIsEqual("Active");
            suspend.AssertIsVisible().AssertIsEnabled();
            resume.AssertIsInvisible();
            archive.AssertIsVisible().AssertIsEnabled();

            suspend.InvokeReturnObject();
            status.AssertTitleIsEqual("Suspended");
            suspend.AssertIsInvisible();
            resume.AssertIsVisible().AssertIsEnabled();
            archive.AssertIsVisible().AssertIsEnabled();
            firstRun.AssertIsNotEmpty();
            nextRun.AssertIsEmpty();
            lastRun.AssertIsNotEmpty();

            resume.InvokeReturnObject();
            status.AssertTitleIsEqual("Active");
            firstRun.AssertIsNotEmpty();
            nextRun.AssertIsNotEmpty();
            lastRun.AssertIsNotEmpty();

            archive.InvokeReturnObject();
            status.AssertTitleIsEqual("Archived");
            suspend.AssertIsInvisible();
            resume.AssertIsInvisible();
            archive.AssertIsInvisible();
            firstRun.AssertIsNotEmpty();
            nextRun.AssertIsEmpty();
            lastRun.AssertIsEmpty();

        }

        [TestMethod()]
        public void RunProcessManually()
        {
            var proc = GetTestService("Batch").GetAction("Find Process Definitions").InvokeReturnCollection("Process2", null).ElementAt(0);
            proc.AssertTitleEquals("Process2");

            var className = proc.GetPropertyByName("Class To Invoke").AssertValueIsEqual("Cluster.Batch.Test.MockSP");
            var instance = proc.GetPropertyByName("Process Instance Id").AssertIsEmpty();

            var run  = proc.GetAction("Run Process Manually");

            var processRun = run.InvokeReturnObject();
            processRun.AssertIsType(typeof(BatchLog)).AssertIsPersistent().AssertTitleEquals("01/01/2000 00:00:00");

            processRun.GetPropertyByName("Process Definition").AssertObjectIsEqual(proc);         
            processRun.GetPropertyByName("When Run").AssertValueIsEqual("01/01/2000 00:00:00");
            processRun.GetPropertyByName("Run Mode").AssertValueIsEqual("Manual");
            processRun.GetPropertyByName("Successful").AssertValueIsEqual("True");
            processRun.GetPropertyByName("Attempt Number").AssertValueIsEqual("1");
            processRun.GetPropertyByName("Outcome").AssertValueIsEqual("MockSP run OK");
            processRun.GetPropertyByName("User").AssertTitleIsEqual("Test");
        }

        [TestMethod()]
        public void RunProcessManuallyThrowsException()
        {
            var proc = GetTestService("Batch").GetAction("Find Process Definitions").InvokeReturnCollection("Process3", null).ElementAt(0);
            proc.AssertTitleEquals("Process3");

            var className = proc.GetPropertyByName("Class To Invoke").AssertValueIsEqual("Cluster.Batch.Test.MockSPThrowsException");
            var instance = proc.GetPropertyByName("Process Instance Id").AssertIsEmpty();

            var run = proc.GetAction("Run Process Manually");

            var processRun = run.InvokeReturnObject();
            processRun.AssertIsType(typeof(BatchLog)).AssertIsPersistent().AssertTitleEquals("01/01/2000 00:00:00");
            processRun.GetPropertyByName("Process Definition").AssertObjectIsEqual(proc);
            processRun.GetPropertyByName("When Run").AssertValueIsEqual("01/01/2000 00:00:00");
            processRun.GetPropertyByName("Run Mode").AssertValueIsEqual("Manual");
            processRun.GetPropertyByName("Successful").AssertValueIsEqual("False");
            processRun.GetPropertyByName("Attempt Number").AssertValueIsEqual("1");
            processRun.GetPropertyByName("Outcome").AssertValueIsEqual("Test Exception");
            processRun.GetPropertyByName("User").AssertTitleIsEqual("Test");
        }

        [TestMethod()]
        public void RunProcessManuallyNotVisibleOnArchivedProcess()
        {
            var proc = GetTestService("Batch").GetAction("Find Process Definitions").InvokeReturnCollection("Process4", null).ElementAt(0);
            proc.AssertTitleEquals("Process4");

            proc.GetPropertyByName("Status").AssertValueIsEqual("Archived");

            var run = proc.GetAction("Run Process Manually").AssertIsInvisible();

        }

        [TestMethod()]
        public void RunPersistentProcessManually()
        {
            var find = GetTestService("Batch").GetAction("Find Process Definitions");
            var proc = find.InvokeReturnCollection("Process5", null).ElementAt(0);
            proc.AssertTitleEquals("Process5");

            var className = proc.GetPropertyByName("Class To Invoke").AssertValueIsEqual("Cluster.Batch.Test.MockPersistentSP");
            var instance = proc.GetPropertyByName("Process Instance Id").AssertValueIsEqual("1");

            var run = proc.GetAction("Run Process Manually");

            var processRun = run.InvokeReturnObject();
            processRun.AssertIsType(typeof(BatchLog)).AssertIsPersistent().AssertTitleEquals("01/01/2000 00:00:00");

            processRun.GetPropertyByName("Process Definition").AssertObjectIsEqual(proc);
            processRun.GetPropertyByName("When Run").AssertValueIsEqual("01/01/2000 00:00:00");
            processRun.GetPropertyByName("Run Mode").AssertValueIsEqual("Manual");
            processRun.GetPropertyByName("Outcome").AssertValueIsEqual("Outcome1");
            processRun.GetPropertyByName("Successful").AssertValueIsEqual("True");
            processRun.GetPropertyByName("Attempt Number").AssertValueIsEqual("1");
            processRun.GetPropertyByName("User").AssertTitleIsEqual("Test");

            //second test
             proc = find.InvokeReturnCollection("Process6", null).ElementAt(0);
            proc.AssertTitleEquals("Process6");

            proc.GetPropertyByName("Class To Invoke").AssertValueIsEqual("Cluster.Batch.Test.MockPersistentSP");
            proc.GetPropertyByName("Process Instance Id").AssertValueIsEqual("2");

            run = proc.GetAction("Run Process Manually");

            processRun = run.InvokeReturnObject();
            processRun.AssertIsType(typeof(BatchLog)).AssertIsPersistent();
            processRun.GetPropertyByName("Outcome").AssertValueIsEqual("Outcome2");

        }

        [TestMethod()]
        public void RunPersistentProcessManuallyWithInvalidId()
        {
            var find = GetTestService("Batch").GetAction("Find Process Definitions");
            var proc = find.InvokeReturnCollection("Process7", null).ElementAt(0);
            proc.AssertTitleEquals("Process7");

            var className = proc.GetPropertyByName("Class To Invoke").AssertValueIsEqual("Cluster.Batch.Test.MockPersistentSP");
            var instance = proc.GetPropertyByName("Process Instance Id").AssertValueIsEqual("100");

            var run = proc.GetAction("Run Process Manually");

            var processRun = run.InvokeReturnObject();
            processRun.AssertIsType(typeof(BatchLog)).AssertIsPersistent();
            processRun.GetPropertyByName("Outcome").AssertValueIsEqual("No instance of type Cluster.Batch.Test.MockPersistentSP with Id =100");
            processRun.GetPropertyByName("Successful").AssertValueIsEqual("False");
        }
    }
}