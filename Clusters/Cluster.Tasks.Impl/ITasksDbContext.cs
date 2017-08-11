using System.Data.Entity;

namespace Cluster.Tasks.Impl
{
    /// <summary>
    /// An external DbContext can implement this interface to ensure that it is covering all types defined in the cluster.
    /// </summary>
    public interface ITasksDbContext
    {
        DbSet<TaskContextLink> TaskContextLinks { get; set; }
        DbSet<TaskHistory> TaskHistories { get; set; }
        DbSet<Task> Tasks { get; set; }
        DbSet<TaskType> TaskTypes { get; set; }
        DbSet<CreateTaskBatchProcess> CreateTaskBatchProcesses { get; set; }
    }
}
