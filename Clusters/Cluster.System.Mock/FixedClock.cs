using System;
using Cluster.System.Api;

namespace Cluster.System.Mock
{
    public class FixedClock : IClock
    {
        #region Constructors

	    public FixedClock()
	    {
			_clock = new DateTime(2000, 1, 1); // TODO: fix this properly (originally 0001)
		}
        public FixedClock(DateTime value) {
            _clock = value;
		}
        #endregion

        private DateTime _clock;

        public void SetClock(DateTime value)
        {
            _clock = value;
        }

        public void Forward(int days)
        {
            _clock = _clock.AddDays(days);
        }

        public DateTime Today() {return _clock.Date; }
        

        public DateTime Now() { return _clock; }
        
    }
}
