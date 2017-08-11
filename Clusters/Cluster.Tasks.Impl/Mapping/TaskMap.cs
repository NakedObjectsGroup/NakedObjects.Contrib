using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Tasks.Impl.Mapping

{
    internal class TaskMap : EntityTypeConfiguration<Task>
    {
        internal TaskMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Tasks", "Tasks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.SuspendedUntil).HasColumnName("SuspendedUntil");
            this.Property(t => t.AssignedToId).HasColumnName("AssignedToId");
            this.Property(t => t.Due).HasColumnName("Due");
            this.Property(t => t.Notes).HasColumnName("Notes");

            // Relationships
            this.HasOptional(t => t.Type);
            

        }
    }
}
