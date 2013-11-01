// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.XmlStore {
    public class DummyServiceManager : IServiceManager {
        #region IServiceManager Members

        public virtual IOid GetOidForService(string name, string typeName) {
            return null;
        }

        public virtual void LoadServices() {}

        public virtual void RegisterService(string name, IOid oid) {}

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}