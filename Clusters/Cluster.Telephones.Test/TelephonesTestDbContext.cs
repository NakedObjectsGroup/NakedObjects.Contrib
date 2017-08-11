using System.Data.Entity;
using Cluster.Telephones.Impl;


namespace Cluster.Telephones.Test
{
    /// <summary>
    /// DbContext used solely for testing the Cluster.Telephones.Impl.
    /// </summary>
    public class TelephonesTestDbContext : DbContext, ITelephonesDbContext
    {
        public TelephonesTestDbContext() : base("ClusterTest") { }


        public DbSet<TelephoneCountryCode> TelephoneCountryCodes { get; set; }
        public DbSet<TelephoneDetails> TelephoneDetails { get; set; }

        public DbSet<MockCountry> MockCountries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new TelephonesTestInitializer());
        }
    }
}