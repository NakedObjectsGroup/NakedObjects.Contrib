using System.Data.Entity;
using Cluster.Forms.Impl;
using Cluster.Names.Impl;
using Cluster.Addresses.Impl;
using Cluster.Tasks.Impl;
using Cluster.Countries.Impl;
using Cluster.Emails.Impl;
using Cluster.Telephones.Impl;
using Cluster.Documents.Impl;
using Cluster.Tasks.Impl.Mapping;
using Cluster.Emails.Impl.Mapping;
using Cluster.Countries.Impl.Mapping;
using Cluster.Accounts.Impl;
using Cluster.Audit.Impl;
using Cluster.Users.Impl;
using Cluster.Users.Impl.Mapping;

namespace Template.DataBase
{
    public class AppDbContext : DbContext,
        IAccountsDbContext,
        IAddressesDbContext,
        IAuditDbContext,
        ICountriesDbContext,
        IDocumentsDbContext,
        IEmailsDbContext,
        IFormsDbContext,
        INamesDbContext,
        ITasksDbContext,
        ITelephonesDbContext,
        IUsersDbContext
    {
        public AppDbContext(string dbName, IDatabaseInitializer<AppDbContext> initializer) : base(dbName)
        {
            Database.SetInitializer(initializer);
        }

        //IAccountsDbContext
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<PersistedBalance> Balances { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<CustomerAccountAccountHolderLink> CustomerAccountAccountHolderLinks { get; set; }


        //IAddressesDbContext
        public DbSet<AbstractAddress> Addresses { get; set; }
        public DbSet<AddressTypeForCountry> AddressTypeForCountries { get; set; }

        //IAuditDbContext
        public DbSet<AuditedEvent> AuditedEvents { get; set; }
        public DbSet<ObjectAuditedEventTargetObjectLink> ObjectAuditedEventTargetObjectLinks { get; set; }

        //ICountriesDbContext
        public DbSet<Country> Countries { get; set; }

        //IDocumentsDbContext
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentHolderLink> DocumentHolderLinks { get; set; }
        public DbSet<ExternalDocumentContentLink> ExternalDocumentContentLinks { get; set; }

        //IEmailsDbContext
        public DbSet<EmailDetails> EmailDetails { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }

        //IFormsDbContext
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<FormSubmissionField> FormFields { get; set; }
        public DbSet<FormDefinition> FormDefinitions { get; set; }

        //INamesDbContext
        public DbSet<AbstractName> Names { get; set; }

        //ISchedulingDbContext
        //public DbSet<BatchProcessDefinition> BatchProcessDefinitions { get; set; }
        //public DbSet<BatchLog> BatchLogs { get; set; }
        //public DbSet<MockPersistentSP> MockPersistentSPs { get; set; }

        // ITasksDbContext       
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskContextLink> TaskContextLinks { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<CreateTaskBatchProcess> CreateTaskBatchProcesses { get; set; }

        //ITelephonesDbContext
        public DbSet<TelephoneDetails> TelephoneDetails { get; set; }
        public DbSet<TelephoneCountryCode> TelephoneCountryCodes { get; set; }

        //TEMP
        //public DbSet<MockAccountHolder> MockAccountHolders { get; set; }
        //public DbSet<SimpleHolder> SimpleHolders { get; set; }

        //IUsersDbContext
        public DbSet<UserDetails> UserDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            MapsForCountriesCluster.AddTo(modelBuilder);
            MapsForEmailsCluster.AddTo(modelBuilder);
            MapsForTasksCluster.AddTo(modelBuilder);
            MapsForUsersCluster.AddTo(modelBuilder);
        }
    }
}