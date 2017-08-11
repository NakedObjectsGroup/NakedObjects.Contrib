using Cluster.System.Api;
using NakedObjects;
namespace Cluster.Documents.Api
{
    /// <summary>
    /// Represents a piece of incoming or outgoing correspondence, or a note.  Roughly equivalent to a item
    /// in a traditional (paper)  file or folder.
    /// </summary>
    [Named("Document")]
    public interface IDocument : IDomainInterface
    {
    }
}
