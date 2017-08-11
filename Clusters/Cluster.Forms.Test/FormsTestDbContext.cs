using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Cluster.Forms.Impl;

namespace Cluster.Forms.Test
{
    public class FormsTestDbContext : DbContext, IFormsDbContext
    {
        public FormsTestDbContext() : base("ClusterTest") { }

        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<FormSubmissionField> FormFields { get; set; }
        public DbSet<FormDefinition> FormDefinitions { get; set; }
    }
}
