using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Mail;
using Cluster.Emails.Impl;

namespace Cluster.Emails.Test
{

    [TestClass()]
    public class EmailsUnitTest : AcceptanceTestCase
    {

        #region Constructors
        public EmailsUnitTest(string name) : base(name) { }

        public EmailsUnitTest() : this(typeof(EmailsUnitTest).Name) { }
        #endregion


        [TestMethod()]
        public virtual void TestPopulationFromAMailMessage()
        {
            var m1 = new MailMessage("\"Richard\" <rpawson@peppard.net>", "scascarini@nakedobjects.org", "TestSubject", "TestBody");

            var email = new Email();

            email.PopulateFrom(m1);

            Assert.AreEqual("\"Richard\" <rpawson@peppard.net>", email.From);
            Assert.AreEqual("scascarini@nakedobjects.org", email.To);
            Assert.AreEqual("TestSubject", email.Subject);
            Assert.AreEqual("TestBody", email.Body);
            //TODO:  Expand to cover attachment etc.
        }

        [TestMethod()]
        public virtual void TestGetAMailMessageFromEmail()
        {
           var email = new Email();
            email.From = "\"Richard\" <rpawson@peppard.net>";
            email.To = "scascarini@nakedobjects.org";
            email.Subject = "TestSubject";
            email.Body = "TestBody";

            var m2 = email.GetMailMessage();

            Assert.AreEqual("\"Richard\" <rpawson@peppard.net>", m2.From.ToString());
            Assert.AreEqual("scascarini@nakedobjects.org", m2.To.ToString());
            Assert.AreEqual("TestSubject", m2.Subject);
            Assert.AreEqual("TestBody", m2.Body);
            //TODO:  Expand to cover attachment etc.
        }
    }
}