using System.Data.Spatial;

namespace Cluster.Addresses.Api
{
    public interface IHasGeoCode
    {

        DbGeography GeoCode { get; }
    }
}
