using System.Data.Entity;
using App.Users.Test;
using Cluster.Users.Impl;

namespace Cluster.Users.Test
{
    public class UsersTestDbContext : UsersDbContext
    {
        public UsersTestDbContext() : base("ClusterTest"){ }

        public DbSet<MockOrganisation> MockOrganisations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new UsersTestInitializer(this));
        }

    }
}