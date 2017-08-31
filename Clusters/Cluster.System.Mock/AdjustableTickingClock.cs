using System;
using Cluster.System.Api;

namespace Cluster.System.Mock
{
    public class AdjustableClock : IClock
    {
        #region Constructors

        public AdjustableClock() {}
        public AdjustableClock(DateTime date)
        {
			SetDate(date);
		}
        #endregion

        private long offsetInTicks = 0;

        public void SetDate(DateTime date)
        {
            offsetInTicks = date.Date.Ticks - DateTime.Today.Ticks;
        }

        public DateTime Today() {return DateTime.Today.AddTicks(offsetInTicks); }
		
        public DateTime Now() { return DateTime.Now.AddTicks(offsetInTicks); }

		public string TodayAsStringRoundTrip()
	    {
		    throw new NotImplementedException();
	    }

	    public string NowAsStringRoundTrip()
	    {
		    throw new NotImplementedException();
	    }

    }
}
