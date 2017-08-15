using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Users.Impl.Mapping
{
    internal class IdentityUserRoleMap : EntityTypeConfiguration<IdentityUserRole>
    {
        public IdentityUserRoleMap()
        {
            // Primary Key
            HasKey(t => new { t.RoleId, t.UserId });

            // Properties
            // Table & Column Mappings
            ToTable("IdentityUserRoles");
        }
    }
}
