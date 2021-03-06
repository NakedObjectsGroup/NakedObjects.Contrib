﻿using Cluster.Names.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cluster.Names.Test
{
    [TestClass]
    public class UnitTests
    {

        [TestMethod, TestCategory("Name Service")]
        public void AppSettingForDefaultNameType()
        {
            string setting = AppSettings.DefaultNameType();
            Assert.AreEqual("Cluster.Names.Impl.WesternName", setting);
        }
    }
}
