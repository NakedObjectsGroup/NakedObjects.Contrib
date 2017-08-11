using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Api
{
    [Named("Account")]
    public interface IAccount : IDomainInterface
    {
        string Code { get; }
        string Currency { get; }
    }
}
