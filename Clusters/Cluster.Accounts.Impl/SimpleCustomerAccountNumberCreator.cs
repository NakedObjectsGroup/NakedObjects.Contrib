using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Accounts.Api;
using NakedObjects;

namespace Cluster.Accounts.Impl
{
    public class SimpleCustomerAccountNumberCreator : ICustomerAccountNumberCreator
    {
        public IDomainObjectContainer Container { set; protected get; }

        public string GetAccountNumberForNewAccount(ICustomerAccount account)
        {
            int number = Container.Instances<CustomerAccount>().Count() + 1;
            string padded = "00000000" + number;
            return padded.Substring(padded.Length - 8);
        }
    }
}
