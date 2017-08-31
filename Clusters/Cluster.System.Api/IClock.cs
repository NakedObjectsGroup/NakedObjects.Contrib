using System;

namespace Cluster.System.Api
{
	/// <summary>
	/// Common point for accessing the clock, to allow it to be mocked for testing
	/// </summary>
	public interface IClock
	{
		/// <summary>
		/// UTC
		/// </summary>
		/// <returns></returns>
		DateTime Today();

		/// <summary>
		/// UTC
		/// </summary>
		/// <returns></returns>
		DateTime Now();

		/// <summary>
		/// See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
		/// </summary>
		/// <returns></returns>
		string TodayAsStringRoundTrip();

		/// <summary>
		/// See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
		/// </summary>
		/// <returns></returns>
		string NowAsStringRoundTrip();
	}
}
