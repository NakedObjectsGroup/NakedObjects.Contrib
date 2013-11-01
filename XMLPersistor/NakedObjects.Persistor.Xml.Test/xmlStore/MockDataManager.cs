// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NUnit.Framework;

namespace NakedObjects.XmlStore {
    public class MockDataManager : IDataManager {
        private readonly IList<Data> actions = new List<Data>();

        #region IDataManager Members

        public virtual bool IsInitialized {
            get { return true; }
        }

        public virtual string DebugData {
            get { return null; }
        }

        public virtual IOid CreateOid(string typeName) {
            return null;
        }

        public virtual CollectionData LoadCollectionData(IOid oid) {
            return null;
        }

        public virtual ObjectData LoadObjectData(IOid oid) {
            return null;
        }

        public virtual void Remove(IOid oid) {}

        public virtual void Save(Data data) {
            actions.Add(data);
        }

        public virtual void Shutdown() {}

        public virtual Data LoadData(IOid oid) {
            return null;
        }

        public void InsertObject(ObjectData data) {}

        public IQueryable<ObjectData> GetInstances(INakedObjectSpecification specification) {
            return null;
        }

        #endregion

        public virtual int NumberOfInstances(ObjectData pattern) {
            return 5;
        }

        public virtual void AssertAction(int i, string action) {
            if (i >= actions.Count) {
                Assert.Fail("No such action " + action);
            }
            // Assert.assertEquals(action, actions.elementAt(i));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}