using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Users.Impl.Mapping
{
    internal class IdentityUserLoginMap : EntityTypeConfiguration<IdentityUserLogin>
    {
        public IdentityUserLoginMap()
        {
            // Primary Key
            HasKey(t => t.UserId);

            // Properties
            // Table & Column Mappings
            ToTable("IdentityUserLogins");
        }
    }
}
