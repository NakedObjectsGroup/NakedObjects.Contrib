using System.Data.Entity;
using Cluster.Tasks.Impl;
using Cluster.Tasks.Impl.Mapping;
using Cluster.Users.Mock;


namespace Cluster.Tasks.Test
{
    public class TasksTestDbContext : DbContext, ITasksDbContext
    {
        public TasksTestDbContext() : base("ClusterTest") { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskContextLink> TaskContextLinks { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<CreateTaskBatchProcess> CreateTaskBatchProcesses { get; set; }
 
        public DbSet<MockUser> MockUsers { get; set; }
        public DbSet<MockRole> MockRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            MapsForTasksCluster.AddTo(modelBuilder);
        }
    }
}