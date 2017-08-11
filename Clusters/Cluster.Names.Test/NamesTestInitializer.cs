using System;
using System.Data.Entity;
using System.IO;
using Cluster.Names.Impl;

namespace Cluster.Names.Test
{
    public class NamesTestInitializer : DropCreateDatabaseAlways<NamesTestDbContext>
    {
        protected override void Seed(NamesTestDbContext context)
        {
            TestNames(context);
            TestIndividuals(context.TestIndividuals);
        }
        public static void TestNames(INamesDbContext context)
        {
            DbSet<AbstractName> dbSet = context.Names;
            NewName(dbSet, WesternNamePrefixes.Dr, "Richard", null, "Pawson");
            NewName(dbSet, WesternNamePrefixes.Mr, "William", "B", "Morris", null, "Bill");
            NewName(dbSet, null, "Marge", null, "Roberts");
        }

        public static WesternName NewName(DbSet<AbstractName> dbSet, WesternNamePrefixes? title, string first, string middle, string last, string suffix = null, string informal = null)
        {
            var name = new WesternName()
            {
                Prefix = title,
                FirstName = first,
                MiddleInitial = middle,
                LastName = last,
                Suffix = suffix,
                InformalFirstName = informal,
                LastModified = DateTime.Now
            };
            name.Searchable = name.SortableName;
            dbSet.Add(name);
            return name;
        }

        public static void TestIndividuals(DbSet<TestIndividual> dbSet)
        {
            NewTestIndividual(dbSet, 2);
        }

        public static TestIndividual NewTestIndividual(DbSet<TestIndividual> dbSet, int nameId)
        {
            var ind = new TestIndividual()
            {
                NameId = nameId
            };
            dbSet.Add(ind);
            return ind;
        }
   
    }
}

