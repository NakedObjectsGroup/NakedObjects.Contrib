using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Tasks.Impl.Mapping
{
    internal class TaskContextLinkMap : EntityTypeConfiguration<TaskContextLink>
    {
        internal TaskContextLinkMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("TaskContextLinks", "Tasks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AssociatedRoleObjectType).HasColumnName("AssociatedRoleObjectType");
            this.Property(t => t.AssociatedRoleObjectId).HasColumnName("AssociatedRoleObjectId");
            //this.Property(t => t.Owner_Id).HasColumnName("Owner_Id");

            // Relationships
            this.HasRequired(t => t.Owner);

        }
    }
}
