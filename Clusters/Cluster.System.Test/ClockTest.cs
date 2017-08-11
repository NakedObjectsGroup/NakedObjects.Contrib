using System;
using Cluster.System.Api;
using Cluster.System.Impl;
using Cluster.System.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cluster.System.Test
{
    [TestClass]
    public class ClockTest
    {
        [TestMethod]
        public void RealClockMatchesDateTime()
        {
            IClock clock = new RealClock();
            DateTime dtnow = DateTime.Now;
            DateTime clockNow = clock.Now();
            Assert.AreEqual(dtnow.ToString(), clockNow.ToString()); //Deliberately accurate only to the second.

            Assert.AreEqual(DateTime.Today, clock.Today());

        }

        [TestMethod]
        public void ProgammableClockDefaultValue()
        {
            FixedClock clock = new FixedClock();
            Assert.AreEqual("01/01/0001 00:00:00", clock.Now().ToString());
            Assert.AreEqual("01/01/0001 00:00:00", clock.Today().ToString());
        }



        [TestMethod]
        public void ProgammableClockInitialValue()
        {
            FixedClock clock = new FixedClock(new DateTime(2013, 7, 28, 14, 08, 30));

           Assert.AreEqual("28/07/2013 14:08:30", clock.Now().ToString());
            Assert.AreEqual("28/07/2013 00:00:00", clock.Today().ToString());
        }


        [TestMethod]
        public void ProgammableClockSetClock()
        {
            FixedClock clock = new FixedClock();

            clock.SetClock(new DateTime(2014, 7, 28, 14, 08, 30));

            Assert.AreEqual("28/07/2014 14:08:30", clock.Now().ToString());
            Assert.AreEqual("28/07/2014 00:00:00", clock.Today().ToString());
        }

        [TestMethod]
        public void ProgammableClockForward()
        {
            FixedClock clock = new FixedClock(new DateTime(2013, 7, 28, 14, 08, 30));
            clock.Forward(2);
            Assert.AreEqual("30/07/2013 14:08:30", clock.Now().ToString());
            Assert.AreEqual("30/07/2013 00:00:00", clock.Today().ToString());
        }


    }
}
