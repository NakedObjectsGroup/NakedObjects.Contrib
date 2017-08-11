using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Cluster.Emails.Impl.Mapping
{
    public static class MapsForEmailsCluster
    {

        public static void AddTo(DbModelBuilder modelBuilder) {

            modelBuilder.Configurations.Add(new EmailMap());
        }
    }
}
