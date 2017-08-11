using System;
using System.Data.Entity;
using System.IO;
using System.Text;
using Cluster.Documents.Impl;
using Cluster.Users.Mock;
using NakedObjects;
using NakedObjects.Core.Context;



namespace Cluster.Documents.Test
{
    public class DocumentsFixture
    {
        #region Injected Services
        
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public  void Install()
        {
            AllTestHolders();
            NakedObjectsContext.ObjectPersistor.EndTransaction();
            NakedObjectsContext.ObjectPersistor.StartTransaction();
            AllNotes();
            AllFileAttachments();
        }

        private SimpleHolder holder1;

        public  void AllTestHolders()
        {

            holder1 = NewTestHolder(); //1
            NewTestHolder(); //2
            NewTestHolder(); //3
            NewTestHolder(); //4
            NewTestHolder(); //5
            NewTestHolder(); //6
        }

        public  void AllNotes()
        {
            NewNote( "Note1");
        }

        public  void AllFileAttachments()
        {
            NewFileAttachment( "TextFile1.txt", "Foo");
        }

        public  SimpleHolder NewTestHolder()
        {
            var c = Container.NewTransientInstance<SimpleHolder>();
            Container.Persist(ref c);
            return c;
        }

        public  Note NewNote(string text)
        {
            var n = Container.NewTransientInstance<Note>();
            n.Text = text;
                n.UserId = 1;
                n.CreatingHolder = holder1;
            Container.Persist(ref n);
            return n;
        }

        public DocumentWithFileAttachment NewFileAttachment(
            string fileName,
            string description = null)
        {
            var fa = Container.NewTransientInstance<DocumentWithFileAttachment>();
            fa.AttContent = Encoding.ASCII.GetBytes("Test Text");
                fa.AttName = fileName;
               fa.AttMime = null; //TODO
               fa.CreatingHolder = holder1;
                fa.Text = description;
                Container.Persist(ref fa);
            return fa;
        }
   }
}

