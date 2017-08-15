using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Users.Impl.Mapping
{
    internal class UserMap : EntityTypeConfiguration<UserDetails>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Users");
        }
    }
}
