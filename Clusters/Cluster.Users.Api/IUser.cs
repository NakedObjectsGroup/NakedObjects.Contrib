using System.Collections.Generic;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Users.Api
{
    [Named("User")]
    public interface IUser : IDomainInterface
    {
        string UserName { get; }

        string FullName { get; }

        string EmailAddress { get; }

        bool CanActFor(IUserOrg organisation);
    }
}
