using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cluster.Accounts.Api;
using NakedObjects;

namespace Cluster.Accounts.Test
{
    [DisplayName("Number Creator")]
    public class MockCustomerAccountNumberCreator : ICustomerAccountNumberCreator
    {
        public IDomainObjectContainer Container { set; protected get; }

        private string nextNumber = null;

        public void SetNextNumber(string nextNumber)
        {
            this.nextNumber = nextNumber;
        }

        public string GetAccountNumberForNewAccount(ICustomerAccount account)
        {
            return nextNumber;
        }
    }
}
