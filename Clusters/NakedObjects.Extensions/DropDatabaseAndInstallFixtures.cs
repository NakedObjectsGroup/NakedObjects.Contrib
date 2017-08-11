using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Helpers
{
    public class DropDatabaseAndInstallFixtures<T> 
        : DropCreateDatabaseAlways<T>
        where T : DbContext
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
