using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Users.Api
{
    /// <summary>
    /// Role interface implemented by any object that can be deemed to have associated users.
    /// Typically this is the organisation that a user works for  - but note that a user
    /// could be associatd with more than one IUserOrg.
    /// In order to be able to add/remove users for an IUserOrg, a user must have the role
    /// UserOrgManager. 
    /// </summary>
    [Named("User Organisation")]
    public interface IUserOrg : IDomainInterface
    {
        string Name { get; }
    }
}
