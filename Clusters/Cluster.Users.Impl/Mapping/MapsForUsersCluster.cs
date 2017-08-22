using System.Data.Entity;

namespace Cluster.Users.Impl.Mapping
{
    public static class MapsForUsersCluster
    {

        public static void AddTo(DbModelBuilder modelBuilder) {

            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new IdentityUserMap());
            modelBuilder.Configurations.Add(new IdentityUserRoleMap());
            modelBuilder.Configurations.Add(new IdentityUserLoginMap());
        }
    }
}
