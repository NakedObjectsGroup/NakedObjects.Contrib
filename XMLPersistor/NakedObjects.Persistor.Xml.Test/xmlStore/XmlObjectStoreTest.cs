// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Adapter;
using NakedObjects.TestSystem;
using NUnit.Framework;

namespace NakedObjects.XmlStore {
    [TestFixture]
    public class XmlObjectStoreTest : TestProxyTestCase {
        private MockDataManager dataManager;
        private TestProxyNakedObject nakedObject;
        private XmlObjectStore objectStore;
        private TestProxySpecification spec;

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            // system
            dataManager = new MockDataManager();
            string xmlDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NakedObjects\xml";

            objectStore = new XmlObjectStore(dataManager, new DummyServiceManager(), xmlDirectory);
            objectStore.Clock = (new TestClock());

            // objects
            spec = new TestProxySpecification(typeof (string));
            spec.SetupFields(new INakedObjectAssociation[0]);
            nakedObject = new TestProxyNakedObject();
            nakedObject.SetupSpecification(spec);
            nakedObject.OptimisticLock = (new SerialNumberVersion(23, null, null));
        }

        [Test]
        public virtual void TestSaveObjectCreatesNewVersion() {
            nakedObject.OptimisticLock = new NullVersion();

            IPersistenceCommand[] commands = new IPersistenceCommand[1];
            commands[0] = objectStore.CreateCreateObjectCommand(nakedObject);
            objectStore.Execute(commands);

            Assert.AreEqual(new FileVersion(null, 1), nakedObject.Version);
        }

        [Test]
        public virtual void TestDeleteObjectRemovesVersion() {
            IPersistenceCommand[] commands = new IPersistenceCommand[1];
            commands[0] = objectStore.CreateDestroyObjectCommand(nakedObject);
            objectStore.Execute(commands);

            Assert.AreEqual(new NullVersion(), nakedObject.Version);
        }

        [Test]
        public virtual void TestUpdateObjectCreatesNewVersion() {
            IPersistenceCommand[] commands = new IPersistenceCommand[1];
            commands[0] = objectStore.CreateSaveObjectCommand(nakedObject);
            objectStore.Execute(commands);

            Assert.AreEqual(new FileVersion(null, 1), nakedObject.Version);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}