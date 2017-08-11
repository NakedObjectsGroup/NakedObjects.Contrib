using System.Data.Entity;
using Cluster.Addresses.Impl;


namespace Cluster.Addresses.Test
{
    /// <summary>
    /// DbContext used solely for testing the Cluster.Addresses.Impl.
    /// </summary>
    public class AddressesTestDbContext : DbContext, IAddressesDbContext
    {
        public AddressesTestDbContext() : base ("ClusterTest") { }

        public DbSet<AbstractAddress> Addresses { get; set; }
        public DbSet<AddressTypeForCountry> AddressTypeForCountries { get; set; }
        public DbSet<MockCountry> MockCountries { get; set; }
    }
}