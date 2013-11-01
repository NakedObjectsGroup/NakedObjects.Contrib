// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.IO;
using System.Linq;

using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Persist;
using NakedObjects.TestSystem;
using NUnit.Framework;

namespace NakedObjects.XmlStore {

    [TestFixture]
    public class XmlDataManagerInstancesTest : TestProxyTestCase {
        private ObjectData[] data;

        protected internal XmlDataManager manager;

        private SerialOid[] oids;
        private ObjectData pattern;
        protected internal int SIZE = 5;


        [SetUp]
        public override void SetUp() {
            base.SetUp();

          
            manager = new XmlDataManager();
            XmlFile.DirectoryName = XmlDataManagerTest.testDir;


            ClearTestDirectory();

            FileVersion.Clock = new TestClock();

            oids = new SerialOid[SIZE];
            data = new ObjectData[SIZE];

            INakedObjectSpecification type = system.GetSpecification(typeof(object));
            pattern = new ObjectData(type, null, new FileVersion("user", 13));
            for (int i = 0; i < SIZE; i++) {
                oids[i] = SerialOid.CreatePersistent(i, typeof(object).FullName);
                data[i] = new ObjectData(type, oids[i], new FileVersion("user", 13));
                manager.InsertObject(data[i]);
            }
        }


        protected internal static void ClearTestDirectory() {
            if (XmlFile.DirectoryInfo.Exists) {
                FileInfo[] files = XmlFile.DirectoryInfo.GetFiles(".xml");
                if (files != null) {
                    foreach (FileInfo f in files) {
                        f.Delete();
                    }
                }
            }
        }

        [TearDown]
        public override void TearDown() {
            system.Shutdown();
            base.TearDown();
        }

        [Test]
        public virtual void TestNumberOfInstances() {
            // todo 
            //Assert.AreEqual(SIZE, manager.NumberOfInstances(pattern));
        }

        [Test]
        public virtual void TestRemove() {
            // todo 
            //SerialOid oid = oids[2];
            //manager.Remove(oid);

            //Assert.AreEqual(SIZE - 1, manager.NumberOfInstances(pattern));

            //IList<ObjectData> instances = manager.GetInstances(pattern);
            //for (int i = 0; i < instances.Count; i++) {
            //    Assert.IsFalse(instances[i] == data[2]);
            //}

            //Assert.IsNull(manager.LoadObjectData(oid));
        }

        [Test]
        public virtual void TestSaveObject() {
            data[2].SetField("Person", SerialOid.CreatePersistent(231, typeof(Person).FullName));
            data[2].SetField("Name", "Fred");
            manager.Save(data[2]);

            Assert.IsTrue(Enumerable.Contains(manager.GetInstances(system.GetSpecification(typeof(object))), data[2]));
            ObjectData read = manager.LoadObjectData(oids[2]);
            Assert.AreEqual(data[2], read);
            Assert.AreEqual(data[2].Value("Name"), read.Value("Name"));
            Assert.AreEqual(data[2].GetField("Person"), read.GetField("Person"));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}