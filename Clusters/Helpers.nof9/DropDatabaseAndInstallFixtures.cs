using System.Data.Entity;

namespace Helpers.nof9
{
    public class DropDatabaseAndInstallFixtures<T> : DropCreateDatabaseAlways<T> where T : DbContext
    {
        public DropDatabaseAndInstallFixtures()
        {
            IsInitialized = true;
        }

        public static bool IsInitialized { get; set; }

        protected override void Seed(T context)
        {
            IsInitialized = false;
        }
    }
}
