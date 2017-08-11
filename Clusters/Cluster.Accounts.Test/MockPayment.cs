using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Accounts.Api;
using NakedObjects;

namespace Cluster.Accounts.Test
{
    public class MockPayment : IPayment
    {      
        public virtual int Id { get; set; }

        public string CurrencyCode()
        {
            return"USD";
        }

        public virtual decimal Amount { get; set; }

        [Title]
        public virtual  string Description { get; set; }

        public virtual string AnalysisCodes { get; set; }
    }
}
