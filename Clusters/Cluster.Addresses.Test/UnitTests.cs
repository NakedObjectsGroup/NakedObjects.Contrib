using System;
using Cluster.Addresses.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cluster.Addresses.Test
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void UnitTestAppSettingForDefaultPostalAddressType()
        {
            string setting = AppSettings.DefaultPostalAddressType();
            Assert.AreEqual("Cluster.Addresses.Impl.UKAddress", setting);
        }

        [TestMethod]
        public void UnitTestAppSettingForDefaultCountry()
        {
            Assert.AreEqual("UK", Cluster.Countries.Api.AppSettings.DefaultCountryISOCode());
        }
    }
}
