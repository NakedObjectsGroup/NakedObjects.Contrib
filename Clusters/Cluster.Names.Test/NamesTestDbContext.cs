using System.Data.Entity;
using Cluster.Names.Impl;


namespace Cluster.Names.Test
{
    /// <summary>
    /// DbContext used solely for testing the Cluster.Names.Impl.
    /// </summary>
    public class NamesTestDbContext : DbContext, INamesDbContext
    {
        public NamesTestDbContext() : base("ClusterTest") { }


        public DbSet<AbstractName> Names { get; set; }
        public DbSet<TestIndividual> TestIndividuals { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new NamesTestInitializer());
        }
    }
}