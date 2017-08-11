using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Accounts.Api
{
    /// <summary>
    /// Allows the algorithm for determing account numbers to be plugged in
    /// </summary>
    public interface ICustomerAccountNumberCreator
    {
        string GetAccountNumberForNewAccount(ICustomerAccount account);
    }
}
