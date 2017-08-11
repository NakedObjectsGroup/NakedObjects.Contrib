using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Api
{

    public interface IPayment : IDomainInterface
    {
        string CurrencyCode();

        decimal Amount { get; }

        string Description { get; }

        string AnalysisCodes { get; }
    }
}
