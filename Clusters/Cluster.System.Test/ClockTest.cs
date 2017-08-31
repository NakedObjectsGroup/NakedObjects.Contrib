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
	    public string UtcAndStringify(DateTime value)
	    {
			return value.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
	    }

		[TestMethod, TestCategory("System_ClockTest")]
        public void RealClockMatchesDateTime()
        {
            IClock clock = new RealClock();
            DateTime dtNow = DateTime.Now.ToUniversalTime();
            DateTime dtToday = DateTime.Today.ToUniversalTime();
			DateTime clockNow = clock.Now();
			DateTime clockToday = clock.Today();

			//Assert.AreEqual(dtNow, clockNow); // fails with different ticks (< 1 s)
			Assert.AreEqual(UtcAndStringify(dtNow), clockNow.ToString(CultureInfo.InvariantCulture)); //Deliberately accurate only to the second.
			Assert.AreEqual(dtToday, clockToday);
			Assert.AreEqual(UtcAndStringify(dtToday), clockToday.ToString(CultureInfo.InvariantCulture));
		}

		[TestMethod, TestCategory("System_ClockTest")]
        public void ProgammableClockDefaultValue()
        {
			var clock = new FixedClock();
			var givenTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc); // from default constructor

			Assert.AreEqual("01/01/2000 00:00:00", clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual("01/01/2000 00:00:00", clock.Today().ToString(CultureInfo.InvariantCulture));
			// similarly
			Assert.AreEqual(UtcAndStringify(givenTime), clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual(UtcAndStringify(givenTime.Date), clock.Today().ToString(CultureInfo.InvariantCulture));
			// also
			Assert.AreEqual(givenTime, clock.Now());
			Assert.AreEqual(givenTime.Date, clock.Today());
		}

		[TestMethod, TestCategory("System_ClockTest")]

        public void ProgammableClockInitialValue()
        {
			var givenTime = new DateTime(2013, 7, 28, 14, 08, 30, DateTimeKind.Utc);
			var clock = new FixedClock(givenTime);

			Assert.AreEqual("07/28/2013 14:08:30", clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual("07/28/2013 00:00:00", clock.Today().ToString(CultureInfo.InvariantCulture));
			// similarly
			Assert.AreEqual(UtcAndStringify(givenTime), clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual(UtcAndStringify(givenTime.Date), clock.Today().ToString(CultureInfo.InvariantCulture));
		}


		[TestMethod, TestCategory("System_ClockTest")]
        public void ProgammableClockSetClock()
        {
			var clock = new FixedClock();

			var givenTime = new DateTime(2013, 7, 28, 14, 08, 30, DateTimeKind.Utc);
			clock.SetClock(givenTime);

			Assert.AreEqual(UtcAndStringify(givenTime), clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual(UtcAndStringify(givenTime.Date), clock.Today().ToString(CultureInfo.InvariantCulture));
        }

		[TestMethod, TestCategory("System_ClockTest")]
        public void ProgammableClockForward()
        {
			var givenTime = new DateTime(2013, 7, 28, 14, 08, 30, DateTimeKind.Utc);
			var clock = new FixedClock(givenTime);
			clock.Forward(2);
			Assert.AreEqual("07/30/2013 14:08:30", clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual("07/30/2013 00:00:00", clock.Today().ToString(CultureInfo.InvariantCulture));
			// similarly
			Assert.AreEqual(UtcAndStringify(givenTime.AddDays(2)), clock.Now().ToString(CultureInfo.InvariantCulture));
			Assert.AreEqual(UtcAndStringify(givenTime.AddDays(2).Date), clock.Today().ToString(CultureInfo.InvariantCulture));
		}

		[TestMethod, TestCategory("System_ClockTest")]
		public void ProgammableClockInitialValueCultureGb()
		{
			var givenTime = new DateTime(2013, 7, 28, 14, 08, 30, DateTimeKind.Utc);
			var clock = new FixedClock(givenTime);

			// Note: these assertions will be affected by machine settings of region, short date and long time formats:
			Assert.AreEqual("2013-07-28 14:08:30", clock.Now().ToString(CultureInfo.CreateSpecificCulture("en-gb")));
			Assert.AreEqual("2013-07-28 00:00:00", clock.Today().ToString(CultureInfo.CreateSpecificCulture("en-gb")));
			// Not equal 
			Assert.AreNotEqual(UtcAndStringify(givenTime), clock.Now().ToString(CultureInfo.CreateSpecificCulture("en-gb")));
			Assert.AreNotEqual(UtcAndStringify(givenTime.Date), clock.Today().ToString(CultureInfo.CreateSpecificCulture("en-gb")));
		}
	}
}
