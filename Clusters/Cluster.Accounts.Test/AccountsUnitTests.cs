using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cluster.Accounts.Api;

namespace Cluster.Accounts.Test
{
    [TestClass]
    public class AccountsUnitTests
    {
        [TestMethod]
        public void TestDefaultCurrencyCode()
        {
            Assert.AreEqual("USD", AppSettings.DefaultCurrencyCode());
        }

        [TestMethod]
        public void ValidCurrencyCodes()
        {
            var codes = AppSettings.ValidCurrencyCodes();
            Assert.AreEqual(3, codes.Length);
            Assert.AreEqual("EUR", codes[0]);
            Assert.AreEqual("GBP", codes[1]);
            Assert.AreEqual("USD", codes[2]);
        }
    }
}
