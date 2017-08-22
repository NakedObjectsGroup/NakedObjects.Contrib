using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Users.Impl.Mapping
{
    internal class IdentityUserMap : EntityTypeConfiguration<IdentityUser>
    {
        public IdentityUserMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            //Don't map derived collections
            //this.Ignore(t => t.Claims);
            //this.Ignore(t => t.Logins);
            //this.Ignore(t => t.Roles);

            // Properties
            // Table & Column Mappings
            ToTable("IdentityUsers");
        }
    }
}
