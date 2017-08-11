using System.Data.Entity;
using Cluster.Emails.Impl;


namespace Cluster.Emails.Test
{
    /// <summary>
    /// DbContext used solely for testing the Cluster.Emails.Impl.
    /// </summary>
    public class EmailsTestDbContext : DbContext, IEmailsDbContext
    {
        public EmailsTestDbContext() : base("ClusterTest") { }

        public DbSet<EmailDetails> EmailDetails { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }

        public DbSet<TestPerson> TestPersons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new EmailsTestInitializer());
        }
     }
}