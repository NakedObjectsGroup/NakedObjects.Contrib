using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Cluster.MultiStep.Test
{
    public class MultiStepTestDbContext : DbContext
    {

        public MultiStepTestDbContext() : base("ClusterTest") { }

        public DbSet<Action1> Action1s { get; set; }
    }
}
