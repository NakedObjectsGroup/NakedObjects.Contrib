using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Audit.Impl;
using Cluster.System.Mock;
using Helpers.nof9;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Audit;
using NakedObjects.Services;
using NakedObjects.Snapshot.Xml.Service;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;

namespace Cluster.Audit.Test
{
	[TestClass]
    public class TestAuditing : ClusterXAT<AuditTestDbContext> //AcceptanceTestCase
    {
		#region Run settings

		protected override Type[] Types
		{
			get
			{
				return new Type[]
				{
					typeof(AuditService),
                    typeof(ObjectAction),
                    typeof(ObjectAuditedEventTargetObjectLink),
                    typeof(ObjectPersisted),
                    typeof(ObjectUpdated),
                    typeof(ServiceAction)
				};
			}
		}

		protected override Type[] Services
		{
			get
			{
				return new Type[]
				{
					typeof(AuditService),
					typeof(SimpleRepository<MockAudited>),
					typeof(AuditContributedActions),
					typeof(PolymorphicNavigator),
					typeof(FixedClock), // TODO: typeof(FixedClock(typeof(DateTime(2000, 1, 1))
					typeof(MockService),
					typeof(XmlSnapshotService)
				};
			}
		}

		//Create main menus here, if they need to be accessed in tests
		protected override IMenu[] MainMenus(IMenuFactory factory)
		{
			return new[] {
				factory.NewMenu<AuditService>(true),

			};
		}


        protected override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);
            var config = new AuditConfiguration<DefaultAuditor>();
            container.RegisterType(typeof(IFacetDecorator), typeof(AuditManager),
                "AuditManager", new ContainerControlledLifetimeManager());
            container.RegisterInstance(typeof(IAuditConfiguration), config,
                 new ContainerControlledLifetimeManager());
        }
        #endregion

        #region Test Methods

        [TestMethod, TestCategory("Audit")]
        public virtual void RecordObjectAction()
        {
            var mock = GetTestService("Mock Auditeds").GetAction("All Instances").InvokeReturnCollection().ElementAt(0);
			mock.AssertTitleEquals("Mock1");

			// TODO: var getLast = mock.GetAction("Last Audited Event", "Auditing");
			var getLast = mock.GetAction("Last Audited Event");
			var last = getLast.InvokeReturnObject(mock);
            Assert.IsNull(last);

            mock.GetAction("Do Something").InvokeReturnObject();
            last = getLast.InvokeReturnObject(mock);
            Assert.IsNotNull(last);
            last.AssertIsType(typeof(ObjectAction)).AssertIsPersistent().AssertIsImmutable();
            Assert.AreEqual(mock, last.GetPropertyByName("Object").ContentAsObject);
            last.GetPropertyByName("Action").AssertValueIsEqual("Do Something");

			//var expectedText = $"Note1\nTest, {UtcAndToStringSortable(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))}";
			var expectedText = $"{UtcAndToString(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))}";
			//last.GetPropertyByName("Date Time").AssertTitleIsEqual("01/01/2000 00:00:00");
			last.GetPropertyByName("Date Time").AssertTitleIsEqual(expectedText);
			last.GetPropertyByName("User Name").AssertValueIsEqual("Test");
        }

		[TestMethod, TestCategory("Audit"), Ignore] //Failing  -  Updated method on Auditor no being called by prop.SetValue 
        public virtual void RecordObjectUpdate()
        {
            var mock = GetTestService("Mock Auditeds").GetAction("All Instances").InvokeReturnCollection().ElementAt(1);
            mock.AssertTitleEquals("Mock2").AssertIsPersistent();

			// TODO: var getLast = mock.GetAction("Last Audited Event", "Auditing");
			var getLast = mock.GetAction("Last Audited Event");
			var last = getLast.InvokeReturnObject(mock);
            Assert.IsNull(last);

            mock.GetPropertyByName("Name").SetValue("Updated Name");

            last = getLast.InvokeReturnObject(mock);
            Assert.IsNotNull(last); // TODO: fails
            last.AssertIsType(typeof(ObjectUpdated));
            Assert.AreEqual(mock, last.GetPropertyByName("Object").ContentAsObject);

			var expectedText = $"{UtcAndToString(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))}";
			last.GetPropertyByName("Date Time").AssertTitleIsEqual(expectedText);
			//last.GetPropertyByName("Date Time").AssertTitleIsEqual("2000-01-01 00:00:00");
			last.GetPropertyByName("User Name").AssertValueIsEqual("Test");
            last.GetPropertyByName("Snapshot").AssertIsNotEmpty(); //Not a proper test!
        }

		[TestMethod, TestCategory("Audit")]
        public virtual void RecordObjectCreate()
        {
            var mock = GetTestService("Mock Auditeds").GetAction("New Instance").InvokeReturnObject();
            mock.GetPropertyByName("Name").SetValue("Foo");
            mock.Save();

			var getLast = mock.GetAction("Last Audited Event");
			var last = getLast.InvokeReturnObject(mock);
            Assert.IsNotNull(last);
                last.AssertIsType(typeof(ObjectPersisted)).AssertIsPersistent().AssertIsImmutable();
            Assert.AreEqual(mock, last.GetPropertyByName("Object").ContentAsObject);
            last.AssertTitleEquals("Create & Save: MockAudited");
			last.GetPropertyByName("User Name").AssertValueIsEqual("Test");

			var expectedText = $"{UtcAndToString(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))}";
			last.GetPropertyByName("Date Time").AssertTitleIsEqual(expectedText);
			//last.GetPropertyByName("Date Time").AssertTitleIsEqual("01/01/2000 00:00:00");
		}

		[TestMethod, TestCategory("Audit")]
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
			serviceEvent.GetPropertyByName("User Name").AssertValueIsEqual("Test");

			var expectedText = UtcAndToString(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			//serviceEvent.GetPropertyByName("Date Time").AssertValueIsEqual(expectedValue); // fails with different ticks precision
			//serviceEvent.GetPropertyByName("Date Time").AssertTitleIsEqual("01/01/2000 00:00:00");
			serviceEvent.GetPropertyByName("Date Time").AssertTitleIsEqual(expectedText);
		}

		#endregion
	}
}
