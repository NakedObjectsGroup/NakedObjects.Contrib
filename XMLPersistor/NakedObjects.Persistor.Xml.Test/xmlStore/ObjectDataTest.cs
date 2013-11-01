// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;

using NakedObjects.Core.Adapter;
using NakedObjects.Core.Persist;
using NakedObjects.TestSystem;
using NUnit.Framework;

namespace NakedObjects.XmlStore {
    [TestFixture]
    public class ObjectDataTest {
        [Test]
        public virtual void TestValueField() {
            FileVersion.Clock = new TestClock();

            TestProxySpecification type = new TestProxySpecification(typeof (string));
            ObjectData objectData = new ObjectData(type, SerialOid.CreatePersistent(13, typeof(string).FullName), new FileVersion(""));

            Assert.AreEqual(null, objectData.Value("name"));
            objectData.SetField("name", "value");
            Assert.AreEqual("value", objectData.Value("name"));

            IEnumerator<string> e = objectData.Fields();
            Assert.IsTrue(e.MoveNext());
            Assert.IsNotNull(e.Current);
            Assert.IsFalse(e.MoveNext());
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}