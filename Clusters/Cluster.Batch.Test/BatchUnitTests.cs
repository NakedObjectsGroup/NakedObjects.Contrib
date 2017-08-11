using System;
using Cluster.Batch.Api;
using Cluster.Batch.Impl;
using Cluster.System.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cluster.Batch.Test
{
    [TestClass]
    public class BatchUnitTests
    {
        [TestMethod]
        public void AScheduledProcessIsValidClassForProcess()
        {
            var process = new BatchProcessDefinition();
            var foo = "Cluster.Batch.Test.MockSP";
            Assert.IsNull(process.Validate(foo, null));
        }

        [TestMethod]
        public void AScheduledProcessNotValidIfInstanceIdSpecified()
        {
            var process = new BatchProcessDefinition();
            var foo = "Cluster.Batch.Test.MockSP";
            Assert.IsNotNull(process.Validate(foo, 1));
        }

        //[TestMethod]
        //public void APersistentScheduledProcessIsValidClassForProcessValidIfInstanceIsSpecified()
        //{
        //    var process = new ProcessDefinition();
        //    var foo = "Cluster.Batch.Test.MockPersistentSP";
        //    Assert.IsNull(process.Validate(foo, 1));
        //}

        [TestMethod]
        public void APersistentScheduledProcessIsValidClassForProcessInvalidIfInstanceNotSpecified()
        {
            var process = new BatchProcessDefinition();
            var foo = "Cluster.Batch.Test.MockPersistentSP";
            Assert.IsNotNull(process.Validate(foo, null));
        }

        [TestMethod]
        public void APersistentScheduledProcessIsValidClassForProcessInvalidIfInstanceOutOfRange()
        {
            var process = new BatchProcessDefinition();
            var foo = "Cluster.Batch.Test.MockPersistentSP";
            Assert.IsNotNull(process.Validate(foo, 10000));
        }

        [TestMethod]
        public void OtherClassInvalidForProcess()
        {
            var process = new BatchProcessDefinition();
            var foo = "Cluster.Batch.Test.MockNotAnSP";
            Assert.IsNotNull(process.Validate(foo, null));
            Assert.IsNotNull(process.Validate(foo, 1));
        }

        [TestMethod]
        public void NonExistantClassNameInvalidForProcess()
        {
            var process = new BatchProcessDefinition();
            var foo = "Cluster.Batch.Test.Yon";
            Assert.IsNotNull(process.Validate(foo, null));
            Assert.IsNotNull(process.Validate(foo, 1));
        }


        [TestMethod]
        public void ValidateDates()
        {
            var process = new BatchProcessDefinition();
            var today = new DateTime(2000, 1, 1);
            var clock = new FixedClock(today);
            process.Clock = clock;
            Assert.IsNull(process.Validate(today, null));
            Assert.IsNull(process.Validate(today.AddDays(1), null));
            Assert.IsNotNull(process.Validate(today.AddDays(-1), null));

            Assert.IsNull(process.Validate(today, today));
            Assert.IsNotNull(process.Validate(today.AddDays(1), today));
            Assert.IsNull(process.Validate(today.AddDays(3), today.AddDays(4)));
        }

        [TestMethod()]
        public virtual void CalculateNextRun()
        {
            var p = new BatchProcessDefinition();
            var dec31_1999 = Date(1999, 1, 31);
            var jan01_2000 =  Date(2000, 1, 1);
            var jan03_2000 =  Date(2000, 1, 3);
            var jan31_2000 =  Date(2000, 1, 31);

            var manual = Frequency.ManualRunsOnly;
            var daily = Frequency.Daily;
            var weekly = Frequency.WeeklyOnMonday;
            var month1 = Frequency.MonthlyOn1stOfMonth;

            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, manual, jan31_2000, null);
            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, manual, null, null);
            TestNextRunDate(p, jan31_2000.AddTicks(1), jan01_2000, manual, jan31_2000, null);
            TestNextRunDate(p, dec31_1999.AddTicks(1), jan01_2000, manual, jan31_2000, null);

            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, daily, jan31_2000, Date(2000, 1, 2));
            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, daily, null,  Date(2000, 1, 2));
            TestNextRunDate(p, jan31_2000.AddTicks(1), jan01_2000, daily, jan31_2000, null);
            TestNextRunDate(p, dec31_1999.AddTicks(1), jan01_2000, daily, jan31_2000, jan01_2000);

            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, weekly, jan31_2000, jan03_2000);
            TestNextRunDate(p, jan03_2000.AddTicks(1), jan01_2000, weekly, jan31_2000, Date(2000, 1, 10));
            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, weekly, null, jan03_2000);
            TestNextRunDate(p, jan31_2000.AddTicks(1), jan01_2000, weekly, jan31_2000, null);
            TestNextRunDate(p, jan31_2000.AddTicks(1), jan01_2000, weekly,  Date(2000, 2, 1), null);
            TestNextRunDate(p, Date(2000, 1, 30).AddTicks(1), jan01_2000, weekly, jan31_2000, jan31_2000);
            TestNextRunDate(p, dec31_1999.AddTicks(1), jan01_2000, weekly, jan31_2000, jan01_2000);


            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, month1, null, Date(2000, 2, 1));
            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, month1, Date(2000, 3, 1), Date(2000, 2, 1));
            TestNextRunDate(p, jan01_2000.AddTicks(1), jan01_2000, month1, jan31_2000, null);
            TestNextRunDate(p, dec31_1999.AddTicks(1), jan01_2000, weekly, jan31_2000, jan01_2000);
        }

        private DateTime Date(int year, int month, int day)
        {
            return new DateTime(year, month, day).Date;
        }


        private void TestNextRunDate(BatchProcessDefinition p, DateTime asOf, DateTime firstRun, Frequency freq, DateTime? lastRun, DateTime? nextRunShouldBe, Status status = Status.Active)
        {
            p.FirstRun = firstRun;
            p.Frequency = freq;
            p.LastRun = lastRun;
            p.CalculateNextRunDate(asOf);
            Assert.AreEqual(nextRunShouldBe, p.NextRun);
        }
    }

   
}
