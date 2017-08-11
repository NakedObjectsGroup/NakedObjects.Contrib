using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;

namespace Cluster.System.Mock
{
    public class FixedClock : IClock
    {
        #region Constructors
        public FixedClock() {}
        public FixedClock(DateTime value) {
            clock = value;
    }
        #endregion

        private DateTime clock = new DateTime();

        public void SetClock(DateTime value)
        {
            clock = value;
        }

        public void Forward(int days)
        {
            clock = clock.AddDays(days);
        }

        public DateTime Today() {return clock.Date; }
        

        public DateTime Now() { return clock; }
        
    }
}
