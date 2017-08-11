using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Accounts.Api
{
    public interface ICustomerAccountsService
    {
        ICustomerAccount FindById(int id);

        /// <summary>
        /// The newAccount name will default to, say 'Sole account', but may be overridden for multiple accounts.
        /// Validated to ensure that it is unique for that account holder.
        /// </summary>
        /// <param name="forHolder"></param>
        /// <param name="newAccountName"></param>
        /// <returns></returns>
        ICustomerAccount CreateNewAccount(
            ICustomerAccountHolder forHolder,
            string currencyCode, 
            decimal openingBalance,
            DateTime asOf, 
            string newAccountName);

        IQueryable<ICustomerAccount> AllAccounts(ICustomerAccountHolder forHolder);

        IQueryable<ICustomerAccount> FindAccounts(ICustomerAccountHolder forHolder, string matchingName);

        ICustomerAccount FindAccountByNumber(string accountNumber);
    }
}
