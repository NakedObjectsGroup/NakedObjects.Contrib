using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Cluster.Users.Impl
{
    public interface IUsersDbContext
    {
         DbSet<UserDetails> UserDetails { get; set; }
    }   
}
