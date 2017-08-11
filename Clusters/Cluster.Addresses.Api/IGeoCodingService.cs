using System.Data.Spatial;
namespace Cluster.Addresses.Api
{
    public interface IGeoCodingService
    {
        DbGeography GetGeoCodeFor(IPostalAddress address);
    }
}
