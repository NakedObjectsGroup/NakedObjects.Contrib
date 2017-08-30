using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Util;
using NakedObjects.Xat;

namespace Helpers.nof9
{
	public abstract class ClusterXAT<TContext> : AcceptanceTestCase
		where TContext : DbContext

	{
		#region Initialization

		private bool _initialized;

		protected void InitializeNakedObjectsFrameworkOnce()
		{
			if (!_initialized)
			{
				InitializeNakedObjectsFramework(this);
				_initialized = true;
			}
		}

		[ClassInitialize]
		public static void ClassInitialize(TestContext tc)
		{
		}

		[ClassCleanup]
		public static void ClassCleanup()
		{
			
		}

		[TestInitialize()]
		public void TestInitialize()
		{
			InitializeNakedObjectsFrameworkOnce();
			StartTest();
		}

		#endregion

		#region Run settings

		protected override string[] Namespaces
		{
			get
			{
				return new string[] { "Cluster" }; //Add top-level namespace(s) that cover the domain model
			}
		}
		
		protected override EntityObjectStoreConfiguration Persistor
		{
			get
			{
				var config = new EntityObjectStoreConfiguration();
				config.UsingCodeFirstContext(Activator.CreateInstance<TContext>);
				return config;
			}
		}
		#endregion

		#region Unity Configuration

		protected override void RegisterTypes(IUnityContainer container)
		{
			base.RegisterTypes(container);
			container.RegisterInstance<IEntityObjectStoreConfiguration>(Persistor, (new ContainerControlledLifetimeManager()));
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Assumes that a SimpleRepository for the type T has been registered in Services
		/// </summary>
		protected ITestObject NewTestObject<T>()
		{
			return GetSimpleRepositoryTestService<T>().GetAction("New Instance").InvokeReturnObject();
		}

		private ITestService GetSimpleRepositoryTestService<T>()
		{
			var name = NameUtils.NaturalName(typeof(T).Name) + "s";
			return GetTestService(name);
		}

		protected ITestObject GetAllInstances<T>(int number)
		{
			return GetSimpleRepositoryTestService<T>().GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
		}

		protected ITestObject GetAllInstances(string simpleRepositoryName, int number)
		{
			return GetTestService(simpleRepositoryName).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
		}

		protected ITestObject GetAllInstances(Type repositoryType, int number)
		{
			return GetTestService(repositoryType).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
		}

		protected ITestObject FindById<T>(int id)
		{
			return GetSimpleRepositoryTestService<T>().GetAction("Find By Key").InvokeReturnObject(id);
		}

		protected ITestObject FindById(string simpleRepositoryName, int id)
		{
			return GetTestService(simpleRepositoryName).GetAction("Find By Key").InvokeReturnObject(id);
		}
        #endregion

        #region Authorization helpers

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
