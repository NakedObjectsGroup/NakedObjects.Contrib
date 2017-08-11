using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Template.DataBase;

namespace Template.SeedData
{
    public class ExampleDbInitializer : DropCreateDatabaseIfModelChanges<ExampleDbContext>
    {
        private ExampleDbContext Context;
        protected override void Seed(ExampleDbContext context)
        {
            this.Context = context;
        }
    }
}
