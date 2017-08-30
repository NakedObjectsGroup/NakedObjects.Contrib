using System;
using System.Globalization;
using Cluster.System.Api;
using Cluster.System.Impl;
using Cluster.System.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cluster.System.Test
{
    [TestClass]
    public class ClockTest
    {
		[TestMethod, TestCategory("System_ClockTest")]
        public void RealClockMatchesDateTime()
        {
            IClock clock = new RealClock();
            DateTime dtnow = DateTime.Now;
            DateTime clockNow = clock.Now();
			Assert.AreEqual(dtnow.ToString(CultureInfo.InvariantCulture), clockNow.ToString(CultureInfo.InvariantCulture)); //Deliberately accurate only to the second.

            Assert.AreEqual(DateTime.Today, clock.Today());

        }

		[TestMethod, TestCategory("System_ClockTest")]
        public void ProgammableClockDefaultValue()
        {
			var clock = new FixedClock();
			Assert.AreEqual("01/01/2000 00:00:00", clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual("01/01/2000 00:00:00", clock.Today().ToString(CultureInfo.InvariantCulture));
        }

		[TestMethod, TestCategory("System_ClockTest")]

        public void ProgammableClockInitialValue()
        {
			var clock = new FixedClock(new DateTime(2013, 7, 28, 14, 08, 30));

			Assert.AreEqual("07/28/2013 14:08:30", clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual("07/28/2013 00:00:00", clock.Today().ToString(CultureInfo.InvariantCulture));
        }


		[TestMethod, TestCategory("System_ClockTest")]
        public void ProgammableClockSetClock()
        {
			var clock = new FixedClock();

            clock.SetClock(new DateTime(2014, 7, 28, 14, 08, 30));

			Assert.AreEqual("07/28/2014 14:08:30", clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual("07/28/2014 00:00:00", clock.Today().ToString(CultureInfo.InvariantCulture));
        }

		[TestMethod, TestCategory("System_ClockTest")]
        public void ProgammableClockForward()
        {
			var clock = new FixedClock(new DateTime(2013, 7, 28, 14, 08, 30));
            clock.Forward(2);
			Assert.AreEqual("07/30/2013 14:08:30", clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual("07/30/2013 00:00:00", clock.Today().ToString(CultureInfo.InvariantCulture));
        }

		[TestMethod, TestCategory("System_ClockTest")]
		public void ProgammableClockInitialValueCultureGb()
		{
			var clock = new FixedClock(new DateTime(2013, 7, 28, 14, 08, 30));

			Assert.AreEqual("28/07/2013 14:08:30", clock.Now().ToString(CultureInfo.CreateSpecificCulture("en-gb")));
			Assert.AreEqual("28/07/2013 00:00:00", clock.Today().ToString(CultureInfo.CreateSpecificCulture("en-gb")));
		}
	}
}
