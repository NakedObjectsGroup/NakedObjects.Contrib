using System.Data.Entity;

namespace Cluster.Users.Impl
{
    public class UsersDbContext : DbContext, IUsersDbContext
    {
        public UsersDbContext(string name) : base(name) { }
        public UsersDbContext() { }

        public DbSet<UserDetails> UserDetails { get; set; }
    }
}