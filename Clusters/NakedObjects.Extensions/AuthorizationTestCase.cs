using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using System;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using System.Security.Principal;

namespace Helpers
{

    public abstract class AuthorizationTestCase : AcceptanceTestCase
    {
        #region Actions
        protected void AssertIsInvisibleByDefault(ITestAction action)
        {
            SetUser("Test");
            action.AssertIsInvisible();
        }

        protected void AssertIsVisibleByDefault(ITestAction action)
        {
            SetUser("Test");
            action.AssertIsVisible();
        }

        protected void AssertIsVisibleByRoles(ITestAction action, params string[] roles)
        {
            foreach (string role in roles)
            {
                SetUser("Test", role);
                action.AssertIsVisible();
            }
        }

        protected void AssertIsVisibleByUser(ITestAction prop, string userName)
        {
            SetUser(userName);
            prop.AssertIsVisible();
        }
        #endregion

        #region Properties
        protected void AssertIsVisibleByRoles(ITestProperty prop, params string[] roles)
        {
            foreach (string role in roles)
            {
                SetUser("Test", role);
                prop.AssertIsVisible();
            }
        }

        protected void AssertIsInvisibleByDefault(ITestProperty prop)
        {
            SetUser("Test");
            prop.AssertIsInvisible();
        }

        protected void AssertIsVisibleByDefault(ITestProperty prop)
        {
            SetUser("Test");
            prop.AssertIsVisible();
        }

        protected void AssertIsVisibleByUser(ITestProperty prop, string userName)
        {
            SetUser(userName);
            prop.AssertIsVisible();
        }

        protected void AssertIsUnmodifiableByDefault(ITestProperty prop)
        {
            SetUser("Test");
            prop.AssertIsUnmodifiable();
        }

        protected void AssertIsModifableByRoles(ITestProperty prop, params string[] roles)
        {
            foreach (string role in roles)
            {
                SetUser("Test", role);
                prop.AssertIsModifiable();
            }
        }

        protected void AssertIsModifiableByUser(ITestProperty prop, string userName)
        {
            SetUser(userName);
            prop.AssertIsModifiable();
        }
        #endregion
    }
}