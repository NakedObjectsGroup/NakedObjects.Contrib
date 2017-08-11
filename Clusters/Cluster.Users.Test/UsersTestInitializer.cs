using NakedObjects;
using System;
using System.Data.Entity;
using System.IO;
using Cluster.Users;
using System.Linq;

using Cluster.Users.Api;
using Cluster.Users.Impl;
using App.Users.Test;
using Cluster.System.Api;
using Cluster.Tasks.Api;
using Cluster.Audit.Api;


namespace Cluster.Users.Test
{
    public class UsersTestInitializer : DropCreateDatabaseAlways<UsersTestDbContext>
    {
       private static string sysAdmin = SystemRoles.SysAdmin;
       private static string taskAssignee = TasksRoles.TaskAssignee;
        private static string auditor = AuditRoles.Auditor;

        public UsersTestInitializer(UsersTestDbContext context)
        {
        }

        protected override void Seed(UsersTestDbContext context)
        {
            TestRoles(context);
            TestUsers(context, context); //Because UsersTestDbContext implements both roles!

            var orgs = context.MockOrganisations;
            NewMockOrganisation(orgs, "Alpha");
            NewMockOrganisation(orgs, "Beta");
            NewMockOrganisation(orgs, "Gamma");
            NewMockOrganisation(orgs, "Delta");
            NewMockOrganisation(orgs, "Epsilon");
        }

        public static MockOrganisation NewMockOrganisation(DbSet<MockOrganisation> dbSet, string name)
        {
            var org = new MockOrganisation()
            {
                Name = name
            };
            dbSet.Add(org);
            return org;
        }


        public static void TestRoles(UsersTestDbContext identityContext)
        {
            NewRole(identityContext, sysAdmin);
             NewRole(identityContext, taskAssignee);
            NewRole(identityContext, auditor);
        }

        private static void NewRole(UsersTestDbContext identityContext, string roleName)
        {

        }

        public static void TestUsers(UsersTestDbContext identityContext,IUsersDbContext userContext)
        {


            NewUser(userContext, "Richard", sysAdmin, taskAssignee);
            NewUser(userContext, "Robbie", sysAdmin);
            NewUser(userContext, "Robert", taskAssignee);
            NewUser(userContext, "Charlie", sysAdmin, taskAssignee);
            NewUser(userContext, "Sven");
            NewUser(userContext, Cluster.Users.Api.Constants.UNKNOWN);
            NewUser(userContext, "Test");

            NewUser(userContext, "Test1");
            NewUser(userContext, "Test2");
            NewUser(userContext, "Test3");
            NewUser(userContext, "Test4");
            NewUser(userContext, "Test5");
        }

        public static void NewUser( IUsersDbContext userContext, string userName, params string[] roles)
        {
            //Create User Details
            UserDetails details = new UserDetails()
            {
                UserName = userName,
                FullName = userName,
                LastModified = DateTime.Now
            };
            userContext.UserDetails.Add(details);
        }
    }
}

