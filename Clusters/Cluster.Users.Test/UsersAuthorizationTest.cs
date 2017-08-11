using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Users.Impl;
using Cluster.System.Mock;
using System;
using Cluster.Users.Test;
using Cluster.Users.Api;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using System.Security.Principal;
using Cluster.System.Api;
using Helpers;

namespace App.Users.Test
{

    [TestClass()]
    public class UsersAuthorizationTest : AuthorizationTestCase
    {
        #region Run configuration

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new UserService(),
                    new FixedClock(new DateTime(2000, 1, 1))
                 );
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

        protected override IAuthorizerInstaller Authorizer
        {
            get
            {
                return new CustomAuthorizerInstaller(
                    new TestDefaultAuthorizer(),
                    new UsersAuthorizer()
                );
            }
        }

        #endregion
        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion

        private string sysAdmin = SystemRoles.SysAdmin;


        [TestMethod()]
        public virtual void UsersService()
        {
            var users = GetTestService("Users");

            var allUsers = users.GetAction("All Users");
            AssertIsInvisibleByDefault(allUsers);
            AssertIsVisibleByRoles(allUsers, sysAdmin);

            var findUserByUserName = users.GetAction("Find User By User Name");
            AssertIsInvisibleByDefault(findUserByUserName);
            AssertIsVisibleByRoles(findUserByUserName, sysAdmin);

            var findUserByRealOrUserName = users.GetAction("Find Users By Real Or User Name");
            AssertIsInvisibleByDefault(findUserByRealOrUserName);
            AssertIsVisibleByRoles(findUserByRealOrUserName, sysAdmin);

            var me = users.GetAction("Me");
            AssertIsVisibleByDefault(me);
        }

        [TestMethod()]
        public virtual void UserNameIsCaseInsensitive()
        {
            SetUser("Test", sysAdmin);
            var user = GetTestService("Users").GetAction("Find User By User Name").InvokeReturnObject("Richard").AssertTitleEquals("Richard");

            ITestProperty prop = user.GetPropertyByName("User Name");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByUser(prop, "Richard");
            AssertIsVisibleByUser(prop, "richard");
        }

        [TestMethod()]
        public virtual void User()
        {
            SetUser("Test", sysAdmin);
            var user =  GetTestService("Users").GetAction("Find User By User Name").InvokeReturnObject("Richard").AssertTitleEquals("Richard");

            //All properties first
            ITestProperty prop = null;

            prop = user.GetPropertyByName("User Name");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin);
            AssertIsVisibleByUser(prop, "Richard");

            prop = user.GetPropertyByName("Full Name");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin);
            AssertIsVisibleByUser(prop, "Richard");
            AssertIsModifiableByUser(prop, "Richard");

            prop = user.GetPropertyByName("Email Address");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin);
            AssertIsVisibleByUser(prop, "Richard");
            
            prop = user.GetPropertyByName("Organisations");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin);
            AssertIsVisibleByUser(prop, "Richard");

            prop = user.GetPropertyByName("Last Modified");
            AssertIsInvisibleByDefault(prop);
            AssertIsVisibleByRoles(prop, sysAdmin);
            AssertIsVisibleByUser(prop, "Richard");
        }
    }
}