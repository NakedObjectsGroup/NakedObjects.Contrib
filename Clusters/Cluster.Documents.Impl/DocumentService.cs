using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cluster.Documents.Api;
using Cluster.Users.Api;
using NakedObjects;
using NakedObjects.Services;
using NakedObjects.Value;


namespace Cluster.Documents.Impl
{
    [DisplayName("Documents")]
    public class DocumentService : IDocumentService
    {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        #endregion

        public IQueryable<IDocument> RecentDocuments(IDocumentHolder holder)
        {
            var links = LinksForHolder(holder);
            return JoinLinksAndDocs(links, AllDocs()).OrderByDescending(x => x.LastModified);
        }

        private IQueryable<Document> AllDocs()
        {
            return Container.Instances<Document>();
        }

        private IQueryable<DocumentHolderLink> LinksForHolder(IDocumentHolder holder)
        {
            string holderType = this.PolymorphicNavigator.GetType(holder);
            int holderId = holder.Id;
            return Container.Instances<DocumentHolderLink>().Where(x => x.AssociatedRoleObjectId == holderId && x.AssociatedRoleObjectType == holderType);
        }

        private IQueryable<Document> JoinLinksAndDocs(IQueryable<DocumentHolderLink> links, IQueryable<Document> docs)
        {
            return from d in docs
                   from l in links
                   where l.Owner.Id == d.Id
                   select d;
        }

        [NotContributedAction(typeof(IUser))]
        public IQueryable<IDocument> FindDocuments(IDocumentHolder holder,
            [Optionally] DocumentType? type,
            [Optionally] DateTime? addedAfter,
            [Optionally] DateTime? addedBefore,
            [Optionally] IUser addedByUser,
            [Optionally] string textSearch)
        {
            var docs = DocumentsMatching(type, addedAfter, addedBefore, addedByUser, textSearch);
            var links = LinksForHolder(holder);
            return JoinLinksAndDocs(links, docs).OrderByDescending(x => x.LastModified);
        }

        private IQueryable<Document> DocumentsMatching(

                            DocumentType? type,
                            DateTime? addedAfter,
                            DateTime? addedBefore,
                            IUser addedByUser,
                            string textSearch)
        {
            int userId = addedByUser != null ? addedByUser.Id : 0;
            return from d in AllDocs()
                   where (type == null || d.Type == type) &&
                         (addedAfter == null || d.LastModified >= addedAfter) &&
                        (addedBefore == null || d.LastModified <= addedBefore) &&
                         (textSearch == null || d.Text.ToUpper().Contains(textSearch.ToUpper()) &&
                            (addedByUser == null || d.UserId == userId)
                        )
                   select d;
        }

        public IQueryable<IDocumentHolder> ListHoldersForThisItem(IDocument item)
        {
            throw new NotImplementedException();
        }

        public IDocument AddNewDocument(IDocumentHolder holder, DocumentType type)
        {
            throw new NotImplementedException();
        }

        #region Add Note
        public IDocument AddNote(IDocumentHolder holder)
        {
            var notes = Container.Instances<Note>().Where(x => x.Status == NoteStatus.Active);
            var links = LinksForHolder(holder);
            var existing = JoinLinksAndDocs(links, notes).FirstOrDefault();
            if (existing != null)
            {
                return existing;
            }
            else
            {
                return NewDocument<Note>(holder);
            }
        }

        internal T NewDocument<T>(IDocumentHolder holder) where T : Document, new()
        {
            var note = Container.NewTransientInstance<T>();
            note.CreatingHolder = holder;
            return note;
        }
        #endregion

        public IDocument AddFileAttachment(IDocumentHolder holder, FileAttachment file, [Optionally] string description)
        {
            var doc = NewDocument<DocumentWithFileAttachment>(holder);
            doc.Text = description;
            doc.AttContent = file.GetResourceAsByteArray();
            doc.AttName = file.Name;
            doc.AttMime = file.MimeType;
            Container.Persist(ref doc);
            return doc;
        }

        [NotContributedAction(typeof(IExtenderProvider))]
        public IDocument AddExternalDocument(IDocumentHolder holder, IExternalDocument content)
        {
            var doc = NewDocument<ExternalDocument>(holder);
            doc.Content = content;
            doc.Text = "External Doc";
            Container.Persist(ref doc);
            return doc;
        }

        
        public IQueryable<Document> AllDocuments()
        {
            return Container.Instances<Document>();
        }
      
    }
}
