using System;
using System.Linq;
using Cluster.Users.Api;

namespace Cluster.Documents.Api
{
    public interface IDocumentService
    {
        IQueryable<IDocument> RecentDocuments(IDocumentHolder holder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="type"></param>
        /// <param name="addedAfter"></param>
        /// <param name="addedBefore"></param>
        /// <param name="addedByUser"></param>
        /// <param name="textSearch">Searches only the text in the document object, not in any file attachment</param>
        /// <returns></returns>
        IQueryable<IDocument> FindDocuments(
            IDocumentHolder holder, 
            DocumentType? type, 
            DateTime? addedAfter,
            DateTime? addedBefore,
            IUser addedByUser,
            string textSearch);


        IQueryable<IDocumentHolder> ListHoldersForThisItem(IDocument item);

        /// <summary>
        /// Returns a transient object of the appropriate type, to be completed and saved.
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IDocument AddNewDocument(IDocumentHolder holder, DocumentType type);

        /// <summary>
        /// Shortcut for adding new document of type Note
        /// </summary>
        /// <param name="holder"></param>
        /// <returns></returns>
        IDocument AddNote(IDocumentHolder holder);


        IDocument AddExternalDocument(IDocumentHolder holder, IExternalDocument content);

    }
}
