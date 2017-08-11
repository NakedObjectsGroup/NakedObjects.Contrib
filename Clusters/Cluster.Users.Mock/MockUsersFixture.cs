using NakedObjects;
using System;
using System.Data.Entity;
using System.IO;
using Cluster.Users.Api;
using Cluster.Users.Mock;


namespace Cluster.Users.Mock
{
    public class MockUsersFixture
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        //Note:  Order in which mock users are persisted is brittle!
        public static int richardId = 1;
        public static int robertId = 4;
        public static int robbieId = 2;
        public static int charlieId = 3;

        public static MockUser Richard;
        public static MockUser Robbie;
        public static MockUser Robert;
        public static MockUser Charlie;

        public  void Install()
        {
            Richard = NewUser( "Richard");
            Robbie = NewUser( "Robbie");
            Robert = NewUser( "Robert");
            Charlie = NewUser( "Charlie");
            NewUser( Constants.UNKNOWN);
            NewUser( "Test");
        }

        public MockUser NewUser( string name)
        {
            MockUser user = Container.NewTransientInstance<MockUser>();
            user.UserName = name;
            user.EmailAddress = "A@b.c";
            Container.Persist(ref user);
            return user;
        }

    }
}

