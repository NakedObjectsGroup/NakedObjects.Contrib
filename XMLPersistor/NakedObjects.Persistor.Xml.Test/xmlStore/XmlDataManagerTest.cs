// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.IO;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.TestSystem;
using NUnit.Framework;

namespace NakedObjects.XmlStore {
    [TestFixture]
    public class XmlDataManagerTest : TestProxyTestCase {
        public static readonly string testDir = "tmp" + Path.DirectorySeparatorChar + "tests";
        protected internal XmlDataManager manager;

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            FileVersion.Clock = new TestClock();


            XmlFile.DirectoryName = testDir;
            manager = new XmlDataManager();
            ClearTestDirectory();
        }

        [TearDown]
        public override void TearDown() {
            system.Shutdown();
        }

        protected internal static void ClearTestDirectory() {
            if (XmlFile.DirectoryInfo.Exists) {
                foreach (FileInfo f in XmlFile.DirectoryInfo.GetFiles("*.xml")) {
                    f.Delete();
                }
            }
        }

        [Test]
        public virtual void TestCreateOid() {
            SerialOid oid = manager.CreateOid(typeof(object).FullName) as SerialOid;
            long last = oid.SerialNo;

            for (int i = 0; i < 3; i++) {
                oid = manager.CreateOid(typeof(object).FullName) as SerialOid;
                Assert.AreNotEqual(last, oid.SerialNo);
                last = oid.SerialNo;
            }
        }

        [Test]
        public virtual void TestWriteReadTypeOidAndVersion() {
            ObjectData data = CreateData(typeof(Role), 99, new FileVersion("user", 19));
            manager.InsertObject(data);

            ObjectData read = manager.LoadObjectData(data.Oid);

            Assert.AreEqual(data.Oid, read.Oid);
            Assert.AreEqual(data.ClassName, read.ClassName);
            Assert.AreEqual(data.Version, read.Version);
        }

        [Test]
        public virtual void TestNextId() {
            long first = manager.NextId();
            Assert.AreNotEqual(first, manager.NextId());
            Assert.AreNotEqual(first + 1, manager.NextId());
            Assert.AreNotEqual(first + 2, manager.NextId());
        }

        [Test]
        public virtual void TestInsertObjectWithFields() {
            ObjectData data = CreateData(typeof(Role), 99, new FileVersion("user", 13));
            data.SetField("Person", SerialOid.CreatePersistent(101, typeof(Person).FullName));
            Assert.IsNotNull(data.GetField("Person"));
            data.SetField("Name", "Harry");
            Assert.IsNotNull(data.Value("Name"));

            manager.InsertObject(data);

            ObjectData read = manager.LoadObjectData(data.Oid);
            Assert.AreEqual(data.Oid, read.Oid);
            Assert.AreEqual(data.ClassName, read.ClassName);

            Assert.AreEqual(data.GetField("Person"), read.GetField("Person"));
            Assert.AreEqual(data.Value("Name"), read.Value("Name"));
        }

        [Test]
        public virtual void TestInsertObjectWithEmptyOneToManyAssociations() {
            ObjectData data = CreateData(typeof(Team), 99, new FileVersion("user", 13));

            data.InitCollection("Members");

            manager.InsertObject(data);

            ObjectData read = manager.LoadObjectData(data.Oid);
            Assert.AreEqual(data.Oid, read.Oid);
            Assert.AreEqual(data.ClassName, read.ClassName);

            IList<IOid> c = read.Elements("Members");
            Assert.IsNull(c);
        }

        [Test]
        public virtual void TestInsertObjectWithOneToManyAssociations() {
            ObjectData data = CreateData(typeof(Team), 99, new FileVersion("user", 13));

            data.InitCollection("Members");
            SerialOid[] oid = new SerialOid[3];
            for (int i = 0; i < oid.Length; i++) {
                oid[i] = SerialOid.CreatePersistent(104 + i, typeof(Team).FullName);
                data.AddElement("Members", oid[i]);
            }
            manager.InsertObject(data);

            ObjectData read = manager.LoadObjectData(data.Oid);
            Assert.AreEqual(data.Oid, read.Oid);
            Assert.AreEqual(data.ClassName, read.ClassName);

            IList<IOid> c = read.Elements("Members");
            for (int i = 0; i < oid.Length; i++) {
                Assert.AreEqual(oid[i], c[i]);
            }
        }


        private static ObjectData CreateData(Type type, long id, IVersion version) {
            INakedObjectSpecification specification = NakedObjectsContext.Reflector.LoadSpecification(type.FullName);
            SerialOid oid = SerialOid.CreatePersistent(id, type.FullName);
            return new ObjectData(specification, oid, version);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}