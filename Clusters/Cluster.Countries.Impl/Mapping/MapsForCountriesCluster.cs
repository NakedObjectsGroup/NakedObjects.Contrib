using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Cluster.Countries.Impl.Mapping
{
    public static class MapsForCountriesCluster
    {

        public static void AddTo(DbModelBuilder modelBuilder) {

            modelBuilder.Configurations.Add(new CountryMap());
        }
    }
}
