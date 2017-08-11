using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cluster.Tasks.Impl.Mapping

{
    internal class TaskTypeMap : EntityTypeConfiguration<TaskType>
    {
        internal TaskTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("TaskTypes", "Tasks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CorrespondingClass).HasColumnName("CorrespondingClass");
            this.Property(t => t.UserCreatable).HasColumnName("UserCreatable");
        }
    }
}
