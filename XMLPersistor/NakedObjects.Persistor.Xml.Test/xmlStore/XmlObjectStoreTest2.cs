// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Adapter;
using NakedObjects.Testing;
using NakedObjects.TestSystem;
using NUnit.Framework;

namespace NakedObjects.XmlStore {
    [TestFixture]
    public class XmlObjectStoreTest2 {
        private MockDataManager dataManager;
        private INakedObject nakedObject;
        private XmlObjectStore objectStore;
  //      private TestProxySpecification spec;

        [SetUp]
        public  void SetUp() {

            // system
            dataManager = new MockDataManager();
            string xmlDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NakedObjects\testing";

            objectStore = new XmlObjectStore(dataManager, new DummyServiceManager(), xmlDirectory);
            objectStore.Clock = (new TestClock());

            ProgrammableTestSystem system = new ProgrammableTestSystem();

            Person person = new Person();
            Role role = new Role();
            role.Person = person;

            nakedObject = system.AdapterFor(person);
  
        }

        /*
         * TODO At the moment this test works using a mock data managed which does little. Need to replace it with a dynamic mock.
         */

        [Test]
        public virtual void TestSaveObject() {
            nakedObject.OptimisticLock = new NullVersion();
/*
            Person person = new Person();
            nakedObject.SetupObject(person);
*/



            IPersistenceCommand[] commands = new IPersistenceCommand[1];
            commands[0] = objectStore.CreateCreateObjectCommand(nakedObject);
            objectStore.Execute(commands);

            IOid oid = nakedObject.Oid;

            objectStore.Reset();

     //       objectStore.GetObject(oid, nakedObject.Specification);

//            IQueryable instances = objectStore.GetInstances(nakedObject.Specification);
        }








        [Test]
        public virtual void TestSaveObjectCreatesNewVersion() {
            nakedObject.OptimisticLock = new NullVersion();

            IPersistenceCommand[] commands = new IPersistenceCommand[1];
            commands[0] = objectStore.CreateCreateObjectCommand(nakedObject);
            objectStore.Execute(commands);

            Assert.AreEqual(new FileVersion(null, 1), nakedObject.Version);

           // dataManager.AssertAction(0, "");
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