using System.Data.Entity;
using Cluster.Countries.Impl;
using Cluster.Countries.Impl.Mapping;

namespace Cluster.Countries.Test
{
    /// <summary>
    /// DbContext used solely for testing the Cluster.Countries.Impl.
    /// </summary>
    public class CountriesTestDbContext : DbContext, ICountriesDbContext
    {
        public CountriesTestDbContext() : base("ClusterTest") { }

        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new CountriesTestInitializer());

            MapsForCountriesCluster.AddTo(modelBuilder);
        }
    }
}