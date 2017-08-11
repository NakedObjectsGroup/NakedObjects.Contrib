using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Accounts.Api;
using NakedObjects;

namespace Cluster.Accounts.Test
{
    public class MockSalesItem : ISalesItem
    {            
        public virtual int Id { get; set; }


        public string CurrencyCode()
        {
            return "USD";
        }

        [Title]
        public virtual decimal Amount { get; set; }

        public virtual string Description { get; set; }

        public virtual string AnalysisCodes { get; set; }

        public void NotifyWhenPaid()
        {
            throw new NotImplementedException();
        }
    }


}
