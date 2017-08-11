using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Cluster.Tasks.Impl.Mapping
{
    public static class MapsForTasksCluster
    {
        public static void AddTo(DbModelBuilder modelBuilder) {
            modelBuilder.Configurations.Add(new TaskMap());
            modelBuilder.Configurations.Add(new TaskTypeMap());
            modelBuilder.Configurations.Add(new TaskHistoryMap());
            modelBuilder.Configurations.Add(new TaskContextLinkMap());
        }
    }
}
