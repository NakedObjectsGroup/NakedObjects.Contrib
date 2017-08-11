using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Emails.Impl;
using System.Data.Entity;
using System;
using Cluster.System.Mock;

namespace Cluster.Emails.Test
{

    [TestClass()]
    public class EmailDetailsTest : AcceptanceTestCase
    {
        #region Run configuration

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new EmailRepository(), 
                    new EmailService(),
                    new SimpleRepository<EmailDetails>(),
                    new TestPersonRepository(),
                    new FixedClock(new DateTime(2000,1,1)));
            }
        }

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                Database.SetInitializer(new EmailsTestInitializer());
                var installer = new EntityPersistorInstaller();
                installer.UsingCodeFirstContext(() => new EmailsTestDbContext());
                return installer;
            }
        }
        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion

          [TestMethod]
        public virtual void EmailAddressValidity()
        {
            var td = GetTestService("Email Detailses").GetAction("New Instance").InvokeReturnObject();
            var email = td.GetPropertyByName("Email Address");
            email.AssertFieldEntryIsValid("ab@gmail.com");
            email.AssertFieldEntryIsValid("\"Abe\" <ab@gmail.com>");
          email.AssertFieldEntryInvalid("a,b@gmail.c");
          email.AssertFieldEntryInvalid("John \"Jr\" Doe <user@host>");
        }

          [TestMethod]
          public virtual void EmailDetailsProperties()
          {
              var ed = GetTestService("Email Detailses").GetAction("New Instance").InvokeReturnObject();
              var email = ed.GetPropertyByName("Email Address").AssertIsMandatory().AssertIsModifiable();
              var desc = ed.GetPropertyByName("Description").AssertIsOptional().AssertIsModifiable();
              var current = ed.GetPropertyByName("Current").AssertIsMandatory().AssertValueIsEqual("True");
              var ver = ed.GetPropertyByName("Verified").AssertIsMandatory().AssertValueIsEqual("False");
              var pref = ed.GetPropertyByName("Preferred").AssertIsMandatory().AssertValueIsEqual("False");

              email.SetValue("ab@go.org");
              ed.AssertCanBeSaved().Save();
              ed.AssertIsPersistent();
              ed.AssertTitleEquals("ab@go.org");
          }

          [TestMethod]
          public void FindEmailDetailsById()
          {
              var find = GetTestService("Email Service").GetAction("Find Email Details By Id");
              var ed1 = find.InvokeReturnObject(1);
              ed1.AssertIsType(typeof(EmailDetails));
              ed1.AssertTitleEquals("ab@MyCompany.com");

              ed1 = find.InvokeReturnObject(2);
              ed1.AssertIsType(typeof(EmailDetails));
              ed1.AssertTitleEquals("bc@MyCompany.com");

          }

          [TestMethod]
          public void CreateEmailDetails()
          {
              var create = GetTestService("Email Service").GetAction("Create New Email Details");
              var ed = create.InvokeReturnObject();
              ed.AssertIsType(typeof(EmailDetails)).AssertIsTransient();
          }


          [TestMethod]
          public void FindEmailDetails()
          {
              var create = GetTestService("Email Service").GetAction("Find Email Details");
              var results = create.InvokeReturnCollection("ab@MyCompany.com");
              results.AssertCountIs(1);

              results = create.InvokeReturnCollection("z@MyCompany");
              results.AssertCountIs(0);

               results = create.InvokeReturnCollection("MyCompany");
              results.AssertCountIs(5);

              results = create.InvokeReturnCollection(".com");
              results.AssertCountIs(7);
          }

          [TestMethod]
          public void FindByEmailAddress()
          {
              var rep = GetTestService("Test Person Repository");
              rep.GetAction("All Test Persons").InvokeReturnCollection().AssertCountIs(5);

              var results = rep.GetAction("Find By Email Address").InvokeReturnCollection("MyCompany");
              results.AssertCountIs(3);
              var tp = results.ElementAt(0).AssertIsType(typeof(TestPerson));
              tp.GetPropertyByName("Email Details").AssertTitleIsEqual("ab@MyCompany.com");
          }
    }
}