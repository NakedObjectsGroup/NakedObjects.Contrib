using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Tasks.Impl.Mapping
{
    internal class TaskHistoryMap : EntityTypeConfiguration<TaskHistory>
    {
        internal TaskHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("TaskHistories", "Tasks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TaskId).HasColumnName("TaskId");
            this.Property(t => t.DateTime).HasColumnName("DateTime");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.AssignedTo).HasColumnName("AssignedTo");
            this.Property(t => t.ChangeMadeBy).HasColumnName("ChangeMadeBy");

            // Relationships
            this.HasRequired(t => t.Task)
                .WithMany(t => t.History)
                .HasForeignKey(d => d.TaskId);

        }
    }
}
