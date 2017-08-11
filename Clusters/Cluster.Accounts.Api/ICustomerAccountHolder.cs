using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using Cluster.Users.Api;
using NakedObjects;

namespace Cluster.Accounts.Api
{

    /// <summary>
    /// Role interface implemented by e.g. Customer or Agent, who can incurr and pay charges.
    /// Note that the same party might have multiple customer accounts.
    /// </summary>
    public interface ICustomerAccountHolder : IUserOrg, IDomainInterface
    {

    }
}
