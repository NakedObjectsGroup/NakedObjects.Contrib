using System.Linq;
namespace Cluster.Addresses.Api
{
    public interface IGeographicDistanceService
    {
        /// <summary>
        /// Returns distance in Km
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns></returns>
        decimal CalculateDistanceBetween(IHasGeoCode location1, IHasGeoCode location2);

        /// <summary>
        /// Returns instances of specified type, ordered by distance (nearest first) from specified location
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        IOrderedQueryable<T> FindNearest<T>(IHasGeoCode location, double? maxDist = null) where T : class, IHasGeoCode;
    }
}
