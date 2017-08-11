using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Accounts.Api
{
    public interface IAccountsService
    {
        void PostTransaction(
            DateTime date,
            string description,
            string currency,
            decimal amount,
            string debitAccountCode,
            string creditAccountCode);
    }
}
