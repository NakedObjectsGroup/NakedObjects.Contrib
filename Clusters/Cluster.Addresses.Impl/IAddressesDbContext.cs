using System.Data.Entity;

namespace Cluster.Addresses.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
    public interface IAddressesDbContext
    {
        DbSet<AbstractAddress> Addresses { get; set; }
        DbSet<AddressTypeForCountry> AddressTypeForCountries { get; set; }
    }
}
