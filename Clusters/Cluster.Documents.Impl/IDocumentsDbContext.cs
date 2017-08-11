using System.Data.Entity;

namespace Cluster.Documents.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
   public interface IDocumentsDbContext
    {
        DbSet<Document> Documents { get; set; }
        DbSet<DocumentHolderLink> DocumentHolderLinks { get; set; }

        DbSet<ExternalDocumentContentLink> ExternalDocumentContentLinks { get; set; }
    }
}
