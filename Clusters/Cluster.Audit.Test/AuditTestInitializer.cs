using System.Data.Entity;

namespace Cluster.Audit.Test
{
    public class AuditTestInitializer : DropCreateDatabaseAlways<AuditTestDbContext>
    {
		protected override void Seed(AuditTestDbContext context)
        {
            NewMock1(context.Mock1s, "Mock1");
            NewMock1(context.Mock1s, "Mock2");
        }

        public static MockAudited NewMock1(DbSet<MockAudited> dbSet, string name)
        {
            MockAudited m = new MockAudited()
            {
                Name = name
            };
            dbSet.Add(m);
            return m;
        }
    }
}
