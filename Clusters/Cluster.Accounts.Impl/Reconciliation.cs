using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Accounts.Impl
{
    /// <summary>
    /// Service that provides methods for marking transactions as reconciled
    /// </summary>
    public class Reconciliation
    {
        public void MarkAsReconciled(IQueryable<TransactionInAccountView> transactions)
        {
            foreach (TransactionInAccountView tx in transactions)
            {
                tx.MarkAsReconciled();
            }
        }
    }
}
