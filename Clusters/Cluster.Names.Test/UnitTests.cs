using System;
using Cluster.Names.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cluster.Names.Test
{
    [TestClass]
    public class UnitTests
    {

        [TestMethod]
        public void AppSettingForDefaultNameType()
        {
            string setting = AppSettings.DefaultNameType();
            Assert.AreEqual("Cluster.Names.Impl.WesternName", setting);
        }
    }
}
