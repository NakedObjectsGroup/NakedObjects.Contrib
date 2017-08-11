using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System;
using NakedObjects;
using Cluster.Users.Impl;
using Cluster.Users.Api;
using Cluster.System.Mock;
using Cluster.System.Api;
using App.Users.Test;

namespace Cluster.Users.Test
{
    [TestClass()]
    public class AbstractUsersTest : AcceptanceTestCase
    {
        #region Run configuration

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(new UserService(),
                    new SimpleRepository<MockOrganisation>());
            }
        }

        protected override IServicesInstaller SystemServices
        {
            get
            {
                return new ServicesInstaller(
                    new FixedClock(new DateTime(2000, 1, 1)),
                    new PolymorphicNavigator());
            }
        }

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                var installer = new EntityPersistorInstaller() { EnforceProxies = false };
                installer.UsingCodeFirstContext(() => new UsersTestDbContext());
                return installer;
            }
        }
        #endregion

        #region Initialize and Cleanup

        protected ITestService Users;
        protected ITestService MockOrgs;


        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
            Users = GetTestService("Users");
            MockOrgs = GetTestService("Mock Organisations");
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion
    }
}