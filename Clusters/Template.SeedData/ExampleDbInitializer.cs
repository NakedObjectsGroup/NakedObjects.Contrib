using System.Data.Entity;
using Template.DataBase;

namespace Template.SeedData
{
    public class ExampleDbInitializer : DropCreateDatabaseIfModelChanges<ExampleDbContext>
    {
        private ExampleDbContext _context;

        protected override void Seed(ExampleDbContext context)
        {
            _context = context;
        }
    }
}
