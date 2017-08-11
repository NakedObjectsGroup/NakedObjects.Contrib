using System.Data.Entity;
using Cluster.Accounts.Impl;

namespace Cluster.Accounts.Test
{
    public class AccountsTestDbContext : DbContext, IAccountsDbContext
    {
        public AccountsTestDbContext() : base("ClusterTest") { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<PersistedBalance> Balances { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<CustomerAccountAccountHolderLink> CustomerAccountAccountHolderLinks { get; set; }

        public DbSet<MockAccountHolder> MockAccountHolders { get; set; }
        public DbSet<MockSalesItem> MockSalesItems { get; set; }
        public DbSet<MockPayment> MockPayments { get; set; }
    }
}