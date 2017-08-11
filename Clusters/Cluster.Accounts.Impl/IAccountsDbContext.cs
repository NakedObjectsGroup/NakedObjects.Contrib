using System;
using System.Data.Entity;

namespace Cluster.Accounts.Impl
{
    public interface IAccountsDbContext
    {
        DbSet<Account> Accounts { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Match> Matches { get; set; }
        DbSet<PersistedBalance> Balances { get; set; }
        DbSet<Period> Periods { get; set; }
        DbSet<CustomerAccountAccountHolderLink> CustomerAccountAccountHolderLinks { get; set; }
    }
}
