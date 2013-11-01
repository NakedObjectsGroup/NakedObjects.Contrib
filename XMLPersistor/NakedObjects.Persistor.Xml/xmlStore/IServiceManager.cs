// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.XmlStore {
    public interface IServiceManager {
        void LoadServices();

        void RegisterService(string name, IOid oid);

        IOid GetOidForService(string name, string typeName);
    }

    // Copyright (c) Naked Objects Group Ltd.
}