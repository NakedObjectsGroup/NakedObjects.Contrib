using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Accounts.Impl
{
    public interface IAccountBalance : IAccountEntry
    {
        new decimal? Credit { get; set; }

        new decimal? Debit { get; set; }

    }
}
