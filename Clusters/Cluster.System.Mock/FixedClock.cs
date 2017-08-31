using System;
using Cluster.System.Api;

namespace Cluster.System.Mock
{
    public class FixedClock : IClock
    {
        #region Constructors

	    public FixedClock()
	    {
			//_clock = new DateTime(2000, 1, 1);
			_clock = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		}
		public FixedClock(DateTime value)
		{
            _clock = value.ToUniversalTime();
		}
        #endregion

        private DateTime _clock;

        public void SetClock(DateTime value)
        {
            _clock = value.ToUniversalTime();
        }

        public void Forward(int days)
        {
            _clock = _clock.ToUniversalTime().AddDays(days);
        }

        public DateTime Today() {return _clock.ToUniversalTime().Date; }
        
        public DateTime Now() { return _clock.ToUniversalTime(); }

		/// <summary>
		/// See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
		/// </summary>
		/// <returns></returns>
		public string TodayAsStringRoundTrip() { return _clock.ToUniversalTime().Date.ToString("O"); }

		/// <summary>
		/// See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
		/// </summary>
		/// <returns></returns>
		public string NowAsStringRoundTrip() { return _clock.ToUniversalTime().ToString("O"); }

	}
}
