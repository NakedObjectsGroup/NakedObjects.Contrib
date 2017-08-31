using System;
using Cluster.Audit.Api;
using Cluster.Audit.Impl;
using Cluster.System.Api;
using Cluster.System.Mock;
using Helpers.nof9;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Authorization;
using NakedObjects.Services;
using NakedObjects.Snapshot.Xml.Service;
using NakedObjects.Xat;
using NakedObjects.Architecture.Component;

namespace Cluster.Audit.Test
{
	[TestClass]
	public class AuditAuthorizationTest : ClusterXAT<AuditTestDbContext> //AcceptanceTestCase
	{
		#region Run settings

		protected override Type[] Types
		{
			get
			{
				return new Type[]
				{
					typeof(AuditService),
					typeof(AuditContributedActions)
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
					typeof(FixedClock),
					typeof(MockService),
					typeof(XmlSnapshotService),
					typeof(SimpleRepository<ServiceAction>),
					typeof(SimpleRepository<ObjectAction>),
					typeof(SimpleRepository<ObjectUpdated>)
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
            var config = new AuthorizationConfiguration<TestDefaultAuthorizer>();
            config.AddNamespaceAuthorizer<AuditAuthorizer>("Cluster.Audit"); // TODO: check this
            container.RegisterInstance<IAuthorizationConfiguration>(config, (new ContainerControlledLifetimeManager()));
            container.RegisterType<IFacetDecorator, AuthorizationManager>("AuthorizationManager", new ContainerControlledLifetimeManager());
        }
        #endregion

        #region Test Methods

        private string sysAdmin = SystemRoles.SysAdmin;
       private string auditor = AuditRoles.Auditor;

        [TestMethod, TestCategory("AuditService")]
		public virtual void AuditService()
		{
			var auditService = GetTestService("Auditing");

			ITestAction action = null;

			action = auditService.GetAction("Recent Audited Events");
			AssertIsInvisibleByDefault(action);
			action.AssertIsInvisible();
			AssertIsVisibleByRoles(action, sysAdmin, auditor);

			action = auditService.GetAction("Find Audited Events");
			AssertIsInvisibleByDefault(action);
			action.AssertIsInvisible();
			AssertIsVisibleByRoles(action, sysAdmin, auditor);
		}

		[TestMethod, TestCategory("AuditService")]
		public virtual void AuditedEvents()
		{
			var sa = GetTestService("Service Actions").GetAction("New Instance").InvokeReturnObject();

			ITestProperty prop = null;
			prop = sa.GetPropertyByName("Service Name");
            AssertIsInvisibleByDefault(prop);

            prop.AssertIsInvisible();
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            prop = sa.GetPropertyByName("Action");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            prop = sa.GetPropertyByName("Parameters");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            prop = sa.GetPropertyByName("Date Time");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            prop = sa.GetPropertyByName("User Name");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            var oa = GetTestService("Object Actions").GetAction("New Instance").InvokeReturnObject();

            prop = oa.GetPropertyByName("Object");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            prop = oa.GetPropertyByName("Action");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            prop = oa.GetPropertyByName("Parameters");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            var ou = GetTestService("Object Updateds").GetAction("New Instance").InvokeReturnObject();

            prop = ou.GetPropertyByName("Object");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);

            prop = ou.GetPropertyByName("Snapshot");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin, auditor);
        }
		#endregion
	}
}
