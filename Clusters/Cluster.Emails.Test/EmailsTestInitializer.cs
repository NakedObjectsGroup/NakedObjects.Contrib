using System;
using System.Data.Entity;
using System.IO;
using Cluster.Emails.Api;
using Cluster.Emails.Impl;


namespace Cluster.Emails.Test
{
    public class EmailsTestInitializer : DropCreateDatabaseAlways<EmailsTestDbContext>
    {
        protected override void Seed(EmailsTestDbContext context)
        {
            AllEmailDetails(context);
            AllTestPersons(context.TestPersons);
        }


        public static void AllEmailDetails(IEmailsDbContext context)
        {
            DbSet<EmailDetails> dbSet = context.EmailDetails;
            NewEmailDetails(dbSet, "ab@MyCompany.com");
            NewEmailDetails(dbSet, "bc@MyCompany.com");
            NewEmailDetails(dbSet, "cd@MyCompany.com");
            NewEmailDetails(dbSet, "de@MyCompany.com");
            NewEmailDetails(dbSet, "ef@MyCompany.com");
            NewEmailDetails(dbSet, "fred@gmail.com");
            NewEmailDetails(dbSet, "joe19@hotmail.com");
            NewEmailDetails(dbSet, "Charlie_9@bt.net");
        }

        public static void AllTestPersons(DbSet<TestPerson> dbSet)
        {
            NewTestPerson(dbSet, 1);
            NewTestPerson(dbSet, 2);
            NewTestPerson(dbSet, 3);
            NewTestPerson(dbSet, 7);
            NewTestPerson(dbSet, 8);
        }

        public static EmailDetails NewEmailDetails(DbSet<EmailDetails> dbSet, string email)
        {
            var c = new EmailDetails()
            {
                EmailAddress = email,
                LastModified = DateTime.Now
            };
            dbSet.Add(c);
            return c;
        }

        public static TestPerson NewTestPerson(DbSet<TestPerson> dbSet, int emailDetailsId)
        {
            var tp = new TestPerson()
            {
                EmailDetailsId = emailDetailsId
            };
            dbSet.Add(tp);
            return tp;
        }
  
   }
}

