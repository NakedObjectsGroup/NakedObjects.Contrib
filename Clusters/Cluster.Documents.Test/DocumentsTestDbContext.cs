using System.Data.Entity;
using Cluster.Documents.Impl;
using Cluster.Users.Mock;

namespace Cluster.Documents.Test
{
    public class DocumentsTestDbContext : DbContext, IDocumentsDbContext
    {
        public DocumentsTestDbContext() : base("ClusterTest") { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentHolderLink> DocumentHolderLinks { get; set; }
        public DbSet<ExternalDocumentContentLink> ExternalDocumentContentLinks { get; set; }

        public DbSet<SimpleHolder> MockHolders { get; set; }
        public DbSet<MockUser> MockUsers { get; set; }
    }
}
