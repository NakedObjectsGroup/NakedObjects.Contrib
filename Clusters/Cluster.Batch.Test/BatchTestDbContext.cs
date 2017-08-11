using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Cluster.Batch.Impl;
using Cluster.Users.Mock;

namespace Cluster.Batch.Test
{
    public class BatchTestDbContext : DbContext, IBatchDbContext
    {
        public BatchTestDbContext() : base("ClusterTest") { }

        public DbSet<BatchProcessDefinition> BatchProcessDefinitions { get; set; }
        public DbSet<BatchLog> BatchLogs { get; set; }

        public DbSet<MockUser> MockUsers { get; set; }
        public DbSet<MockPersistentSP> MockPersistentSPs { get; set; }
    }
}
