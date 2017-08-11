using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System;
using NakedObjects;
using Cluster.Users.Impl;
using Cluster.Users.Api;
using Cluster.System.Mock;
using Cluster.System.Api;

namespace Cluster.Users.Test
{

    [TestClass()]
    public class UserServiceTest : AbstractUsersTest
    {
        [TestMethod]
        public void FindSingleUserByUserName()
        {
            var find = GetTestService("Users").GetAction("Find User By User Name");
           var rich   = find.InvokeReturnObject("Richard");
           rich.AssertTitleEquals("Richard");
        }

        [TestMethod]
        public void FindSingleUserByUserNameNotCaseSensitive()
        {
            var find = GetTestService("Users").GetAction("Find User By User Name");
            var rich = find.InvokeReturnObject("RICHARD");
            rich.AssertTitleEquals("Richard");
        }

        [TestMethod]
        public void FindSingleUserByUserNameDoesNotDoPartialMatch()
        {
            var find = GetTestService("Users").GetAction("Find User By User Name");
            var rich = find.InvokeReturnObject("RICH");
            Assert.IsNull(rich);
        }

        [TestMethod]
        public void FindUsersByRealOrUserName()
        {
            var find = GetTestService("Users").GetAction("Find Users By Real Or User Name");
            var users = find.InvokeReturnCollection("ROB");
            users.AssertCountIs(2);
            users.ElementAt(0).AssertTitleEquals("Robbie");
            users.ElementAt(1).AssertTitleEquals("Robert");
        }

        [TestMethod]
        public void NoMatch()
        {
            var find = GetTestService("Users").GetAction("Find User By User Name");
            var zzz = find.InvokeReturnObject("zzz");
            Assert.IsNull(zzz);
        }

        [TestMethod]
        public void Me()
        {
            SetUser("Robbie");
            var user = GetTestService("Users").GetAction("Me").InvokeReturnObject();
            user.AssertTitleEquals("Robbie");
        }


    }
}