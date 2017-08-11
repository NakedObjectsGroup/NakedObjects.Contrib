using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Cluster.Documents.Impl;
using Cluster.Documents.Api;
using Cluster.Users.Mock;
using Cluster.System.Mock;
using System;
using Helpers;

namespace Cluster.Documents.Test
{

    [TestClass()]
    public class DocumentsXAT : ClusterXAT<DocumentsTestDbContext, DocumentsFixture>
    {

        #region Run configuration
        //Set up the properties in this region exactly the same way as in your Run class

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new SimpleRepository<Note>(), 
                    new SimpleRepository<SimpleHolder>(), 
                    new SimpleRepository<DocumentWithFileAttachment>(),
                    new DocumentService(),
                    new PolymorphicNavigator(),
                    new MockUserService(),
                    new FixedClock(new DateTime(2000, 1, 1)));
            }
        }


        protected override IFixturesInstaller Fixtures
        {
            get
            {
                return new FixturesInstaller(
                    new MockUsersFixture(),
                    new DocumentsFixture()
                    );
            }
        }
        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion


        [TestMethod()]
        public virtual void RecentDocumentsAssociatedWithHolder()
        {
            var holder = GetTestService("Simple Holders").GetAction("Find By Key").InvokeReturnObject(2);
            holder.AssertIsType(typeof(SimpleHolder));

            var newNote = holder.GetAction("Add Note", "Documents").InvokeReturnObject(holder);
            var text = newNote.GetPropertyByName("Text").SetValue("Foo");
            newNote.Save();

            var recent = holder.GetAction("Recent Documents", "Documents").InvokeReturnCollection(holder);
            recent.AssertCountIs(1);
            var doc = recent.ElementAt(0);
            doc.AssertIsType(typeof(Note));
        }

        [TestMethod()]
        public virtual void FindDocumentsAssociatedWithHolder()
        {
            var holder = GetTestService("Simple Holders").GetAction("Find By Key").InvokeReturnObject(3);
            holder.AssertIsType(typeof(SimpleHolder));

            var newNote = holder.GetAction("Add Note", "Documents").InvokeReturnObject(holder);
            var text = newNote.GetPropertyByName("Text").SetValue("Foo");
            newNote.Save();

            var recent = holder.GetAction("Find Documents", "Documents").InvokeReturnCollection(holder, DocumentType.Note, null, null, null, null);
            recent.AssertCountIs(1);
            var doc = recent.ElementAt(0);
            doc.AssertIsType(typeof(Note));
        }

        #region Notes

        [TestMethod]
        public void PersistedNote()
        {
            SetUser("Richard");
            var note = GetTestService("Notes").GetAction("Find By Key").InvokeReturnObject(1);
            note.GetPropertyByName("Text").AssertIsUnmodifiable().AssertValueIsEqual("Note1\nTest, 01/01/2000 00:00:00");

            note.GetPropertyByName("Id").AssertIsInvisible();

            note.GetPropertyByName("Creating Holder").AssertIsInvisible();

            note.GetPropertyByName("Holder Links").AssertIsInvisible();

            note.GetPropertyByName("Status").AssertIsUnmodifiable().AssertValueIsEqual("Active");

            note.GetPropertyByName("User").AssertIsVisible().AssertIsUnmodifiable().AssertTitleIsEqual("Test");

            note.GetPropertyByName("Last Modified").AssertIsVisible().AssertIsUnmodifiable();
        }

        [TestMethod()]
        public virtual void AddNoteToHolderWhereNonePrevious()
        {
            var holder = GetTestService("Simple Holders").GetAction("Find By Key").InvokeReturnObject(6);
            holder.AssertIsType(typeof(SimpleHolder)).AssertIsPersistent();

            var newNote = holder.GetAction("Add Note", "Documents").InvokeReturnObject(holder);
            newNote.AssertIsType(typeof(Note)).AssertIsTransient().AssertTitleEquals("Notes");
            var text = newNote.GetPropertyByName("Text");
            text.AssertIsEmpty().AssertIsMandatory();
            text.SetValue("Foo");

            var userName = newNote.GetPropertyByName("User");
            userName.AssertIsInvisible();

            var timeStamp = newNote.GetPropertyByName("Last Modified");
            timeStamp.AssertIsUnmodifiable();

            newNote.Save();
            newNote.AssertTitleEquals("Notes");

            timeStamp.AssertIsVisible().AssertIsNotEmpty();
            userName.AssertIsVisible().AssertValueIsEqual("Test");

            var links = newNote.GetPropertyByName("Holder Links").AssertIsInvisible();
            var link = links.ContentAsCollection.AssertCountIs(1).ElementAt(0);
            Assert.AreEqual(holder, link.GetPropertyByName("Associated Role Object").ContentAsObject);
        }

        [TestMethod()]
        public virtual void AddNoteOntoPrevious()
        {
            var holder = GetTestService("Simple Holders").GetAction("Find By Key").InvokeReturnObject(4);
            holder.AssertIsType(typeof(SimpleHolder));

            var newNote = holder.GetAction("Add Note", "Documents").InvokeReturnObject(holder);
            var text = newNote.GetPropertyByName("Text").SetValue("Foo");
            newNote.Save();


            var oldNote = holder.GetAction("Add Note", "Documents").InvokeReturnObject(holder);
            oldNote.AssertIsPersistent();
            text = oldNote.GetPropertyByName("Text").AssertIsUnmodifiable();
            var lines = text.Title.Split('\n');
            Assert.AreEqual(2, lines.Count());
            Assert.AreEqual("Foo", lines[0]);
            Assert.IsTrue(lines[1].StartsWith("Test,"));

            oldNote.GetAction("Add To Note").AssertIsEnabled().InvokeReturnObject("Bar");

            lines = text.Title.Split('\n');
            Assert.AreEqual(5, lines.Count());
            Assert.AreEqual("", lines[2]);
            Assert.AreEqual("Bar", lines[3]);
            Assert.IsTrue(lines[4].StartsWith("Test,"));

            oldNote.GetAction("Add To Note").AssertIsEnabled().InvokeReturnObject("Yon");

            lines = text.Title.Split('\n');
            Assert.AreEqual(8, lines.Count());
            Assert.AreEqual("", lines[5]);
            Assert.AreEqual("Yon", lines[6]);
            Assert.IsTrue(lines[7].StartsWith("Test,"));
        }

        [TestMethod]
        public void FinishThisNoteAndStartANewOne()
        {
            var holder = GetTestService("Simple Holders").GetAction("Find By Key").InvokeReturnObject(5);

            var note = holder.GetAction("Add Note", "Documents").InvokeReturnObject(holder);
            var text = note.GetPropertyByName("Text").SetValue("Foo");
            note.Save();

            var status = note.GetPropertyByName("Status");
            status.AssertIsUnmodifiable().AssertValueIsEqual("Active");
            var addToNote = note.GetAction("Add To Note").AssertIsEnabled();

            var finish = note.GetAction("Finish This Note").AssertIsEnabled();
            finish.InvokeReturnObject();
            status.AssertValueIsEqual("Finished");
            addToNote.AssertIsDisabled();
            finish.AssertIsInvisible();

            note = holder.GetAction("Add Note", "Documents").InvokeReturnObject(holder);
            note.AssertIsTransient();
        }

        #endregion
        #region File Attachments

        [TestMethod]
        public void PersistedFA()
        {
            var fa = GetTestService("Document With File Attachments").GetAction("All Instances").InvokeReturnCollection().ElementAt(0);

            fa.AssertIsType(typeof(DocumentWithFileAttachment));

            fa.GetPropertyByName("Description").AssertIsModifiable().AssertValueIsEqual("Foo");

            var att = fa.GetPropertyByName("Attachment");
            att.AssertTitleIsEqual("TextFile1.txt");
        }

        [TestMethod()]
        public virtual void AddFileAttachment()
        {
            var holder = GetTestService("Simple Holders").GetAction("Find By Key").InvokeReturnObject(1);
            holder.AssertIsType(typeof(SimpleHolder));

            var add = holder.GetAction("Add File Attachment", "Documents");

            add.Parameters[0].AssertIsNamed("Holder").AssertIsMandatory();
            add.Parameters[1].AssertIsNamed("File").AssertIsMandatory();
            add.Parameters[2].AssertIsNamed("Description").AssertIsOptional();
        }
        #endregion
    }
}