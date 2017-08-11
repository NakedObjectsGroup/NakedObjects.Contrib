using System;
using Cluster.Countries.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cluster.Countries.Test
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void UnitTestAppSettingForDefaultCountryCode()
        {
            string setting = AppSettings.DefaultCountryISOCode();
            Assert.AreEqual("UK", setting);
        }
    }
}
