using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Xat;
using System.Data.Entity;
using System;

namespace Helpers
{
    public class ClusterXAT<TDbContext, TFixture> : AcceptanceTestCase
        where TDbContext: DbContext, new()
        where TFixture : new()
    {

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                var installer = new EntityPersistorInstaller();
                Database.SetInitializer(new DropDatabaseAndInstallFixtures<TDbContext>());
                installer.UsingCodeFirstContext(() => new TDbContext());
                installer.IsInitializedCheck = () => DropDatabaseAndInstallFixtures<TDbContext>.IsInitialized;
                return installer;
            }
        }

        protected override IFixturesInstaller Fixtures
        {
            get
            {
                return new FixturesInstaller(new TFixture());
            }
        }

		#region DateTime Helpers

		public string UtcAndToStringRoundTrip(DateTime value)
		{
			return value.ToUniversalTime().ToString("O");
		}

		/// <summary>
		/// XAT assertions with the current machine's regional settings
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string UtcAndToString(DateTime value)
		{
			return value.ToUniversalTime().ToString();
		}
		#endregion
	}
}
