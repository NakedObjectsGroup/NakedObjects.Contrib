using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Countries.Api
{
    /// <summary>
    /// Defines a country. This is a result interface.
    /// </summary>
    [Named("Country")]
    public interface ICountry : IDomainInterface
    {
        string Name { get; }
        string ISOCode { get; }
    }
}
