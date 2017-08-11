using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NakedObjects;

namespace Cluster.Accounts.Impl
{
    [Named("Entry")]
    public interface IAccountEntry
    {
        string Description { get; }

        [Mask("d")]
        DateTime Date { get; }

        decimal? Debit { get;  }

        decimal? Credit { get; } 
    }
}
