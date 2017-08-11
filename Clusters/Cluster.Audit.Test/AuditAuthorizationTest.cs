using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using Cluster.Audit.Impl;
using Cluster.System.Api;
using Cluster.Audit.Api;
using Helpers;

namespace Cluster.Audit.Test
{

    [TestClass()]
    public class AuditAuthorizationTest : AuthorizationTestCase
    {
        #region Run configuration
        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new AuditService(),
                    new SimpleRepository<ServiceAction>(),
                    new SimpleRepository<ObjectAction>(),
                    new SimpleRepository<ObjectUpdated>());
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

        protected override IAuthorizerInstaller Authorizer
        {
            get
            {
                return new CustomAuthorizerInstaller(
                    new TestDefaultAuthorizer(),
                    new AuditAuthorizer()
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

        private string sysAdmin = SystemRoles.SysAdmin;
        private string auditor = AuditRoles.Auditor;

        [TestMethod()]
        public virtual void AuditService()
        {
            var auditService = GetTestService("Auditing");

            ITestAction action = null;

            action = auditService.GetAction("Recent Audited Events");
            AssertIsInvisibleByDefault(action);
            AssertIsVisibleByRoles(action, sysAdmin, auditor);

            action = auditService.GetAction("Find Audited Events");
            AssertIsInvisibleByDefault(action);
            AssertIsVisibleByRoles(action, sysAdmin, auditor);
        }

        [TestMethod()]
        public virtual void AuditedEvents()
        {
            var sa = GetTestService("Service Actions").GetAction("New Instance").InvokeReturnObject();

            ITestProperty prop = null;
            prop = sa.GetPropertyByName("Service Name");
            AssertIsInvisibleByDefault(prop);
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
    }
}