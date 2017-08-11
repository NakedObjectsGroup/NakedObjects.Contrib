using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Cluster.Audit.Impl;

namespace Cluster.Audit.Test
{
    public class AuditTestDbContext : DbContext, IAuditDbContext
    {
        public AuditTestDbContext() : base("ClusterTest") { }

        public DbSet<AuditedEvent> AuditedEvents { get; set; }
        public DbSet<ObjectAuditedEventTargetObjectLink> ObjectAuditedEventTargetObjectLinks { get; set; }

        public DbSet<MockAudited> Mock1s { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new AuditTestInitializer());
        }


    }
}
