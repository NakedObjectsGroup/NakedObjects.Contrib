using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Audit.Impl;
using System;
using Cluster.System.Mock;
using NakedObjects.Reflector.Audit;
using NakedObjects.Snapshot;

namespace Cluster.Audit.Test
{

    [TestClass()]
    public class TestAuditing : AcceptanceTestCase
    {

        #region Constructors
        public TestAuditing(string name) : base(name) { }

        public TestAuditing() : this(typeof(TestAuditing).Name) { }
        #endregion

        #region Run configuration
        //Set up the properties in this region exactly the same way as in your Run class

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new AuditService(), 
                    new SimpleRepository<MockAudited>(),
                    new AuditContributedActions(),
                    new PolymorphicNavigator(),
                    new FixedClock(new DateTime(2000,1,1)),
                    new MockService(),
                    new XmlSnapshotService());
            }
        }

        protected override IAuditorInstaller Auditor
        {
            get
            {
                return new AuditInstaller(new DefaultAuditor());
            }
        }


        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                var installer = new EntityPersistorInstaller();
                installer.UsingCodeFirstContext(() => new AuditTestDbContext());
                return installer;
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
        public virtual void RecordObjectAction()
        {
            var mock = GetTestService("Mock Auditeds").GetAction("All Instances").InvokeReturnCollection().ElementAt(0);
           mock.AssertTitleEquals("Mock1");

            var getLast = mock.GetAction("Last Audited Event", "Auditing");
            var last = getLast.InvokeReturnObject(mock);
            Assert.IsNull(last);

            mock.GetAction("Do Something").InvokeReturnObject();
            last = getLast.InvokeReturnObject(mock);
            Assert.IsNotNull(last);
            last.AssertIsType(typeof(ObjectAction)).AssertIsPersistent().AssertIsImmutable();
            Assert.AreEqual(mock, last.GetPropertyByName("Object").ContentAsObject);
            last.GetPropertyByName("Action").AssertValueIsEqual("Do Something");
            last.GetPropertyByName("Date Time").AssertTitleIsEqual("01/01/2000 00:00:00");
            last.GetPropertyByName("User Name").AssertValueIsEqual("Test");
        }

        [TestMethod()]
        public virtual void RecordObjectUpdate()
        {
            var mock = GetTestService("Mock Auditeds").GetAction("All Instances").InvokeReturnCollection().ElementAt(1);
            mock.AssertTitleEquals("Mock2").AssertIsPersistent();

            var getLast = mock.GetAction("Last Audited Event", "Auditing");
            var last = getLast.InvokeReturnObject(mock);
            Assert.IsNull(last);

            mock.GetPropertyByName("Name").SetValue("Updated Name");

            last = getLast.InvokeReturnObject(mock);
            Assert.IsNotNull(last);
            last.AssertIsType(typeof(ObjectUpdated));
            Assert.AreEqual(mock, last.GetPropertyByName("Object").ContentAsObject);
            last.GetPropertyByName("Date Time").AssertTitleIsEqual("01/01/2000 00:00:00");
            last.GetPropertyByName("User Name").AssertValueIsEqual("Test");
            last.GetPropertyByName("Snapshot").AssertIsNotEmpty(); //Not a proper test!
        }

        [TestMethod()] 
        public virtual void RecordObjectCreate()
        {
            var mock = GetTestService("Mock Auditeds").GetAction("New Instance").InvokeReturnObject();
            mock.GetPropertyByName("Name").SetValue("Foo");
            mock.Save();

            var getLast = mock.GetAction("Last Audited Event", "Auditing");
            var last = getLast.InvokeReturnObject(mock);
            Assert.IsNotNull(last);
                last.AssertIsType(typeof(ObjectPersisted)).AssertIsPersistent().AssertIsImmutable();
            Assert.AreEqual(mock, last.GetPropertyByName("Object").ContentAsObject);
            last.AssertTitleEquals("Create & Save: MockAudited");
            last.GetPropertyByName("Date Time").AssertTitleIsEqual("01/01/2000 00:00:00");
            last.GetPropertyByName("User Name").AssertValueIsEqual("Test");
        }

        [TestMethod()]
        public virtual void RecordServiceAction()
        {
            var recentAct = GetTestService("Auditing").GetAction("Recent Audited Events");
           var recent =  recentAct.InvokeReturnCollection();
           int startCount = recent.Count();
 
            GetTestService("Mock Service").GetAction("Do Something").InvokeReturnObject();
            recent = recentAct.InvokeReturnCollection();
            recent.AssertCountIs(startCount + 1);

            var serviceEvent = recent.ElementAt(startCount); //i.e. penultimate one
            serviceEvent.AssertIsType(typeof(ServiceAction)).AssertIsImmutable().AssertIsPersistent();
            serviceEvent.GetPropertyByName("Service Name").AssertValueIsEqual("Mock Service");
            serviceEvent.GetPropertyByName("Action").AssertValueIsEqual("Do Something");
            serviceEvent.GetPropertyByName("Date Time").AssertTitleIsEqual("01/01/2000 00:00:00");
            serviceEvent.GetPropertyByName("User Name").AssertValueIsEqual("Test");
        }
    }
}