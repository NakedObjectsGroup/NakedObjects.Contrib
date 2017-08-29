using System.Data.Entity;

namespace Cluster.MultiStep.Test
{
    public class MultiStepTestDbContext : DbContext
    {

        public MultiStepTestDbContext() : base("ClusterTest") { }

        public DbSet<Action1> Action1s { get; set; }
    }
}
