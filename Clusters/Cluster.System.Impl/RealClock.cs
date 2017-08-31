using System;
using Cluster.System.Api;

namespace Cluster.System.Impl
{
	/// <summary>
	/// This is the implementation of IClock that should be registered as a system service
	/// for normal operation; it merely wraps the DateTime static functions.
	/// </summary>
	public class RealClock : IClock
	{
		public DateTime Today() { return DateTime.Today.ToUniversalTime(); }

		public DateTime Now() {return DateTime.Now.ToUniversalTime(); }

		/// <summary>
		/// See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
		/// </summary>
		/// <returns></returns>
		public string TodayAsStringRoundTrip() { return DateTime.Today.ToUniversalTime().ToString("O"); }

		/// <summary>
		/// See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
		/// </summary>
		/// <returns></returns>
		public string NowAsStringRoundTrip() { return DateTime.Now.ToUniversalTime().ToString("O"); }

	}
}
