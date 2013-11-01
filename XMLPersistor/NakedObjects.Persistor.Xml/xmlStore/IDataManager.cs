// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.XmlStore {
    public interface IDataManager {
        string DebugData { get; }
        bool IsInitialized { get; }
        IOid CreateOid(string typeName);

        /// <summary>
        ///     Save the data for an object and adds the reference to a list of instances
        /// </summary>
        void InsertObject(ObjectData data);

        /// <summary>
        ///     Loads in data for a collection for the specified identifier
        /// </summary>
        CollectionData LoadCollectionData(IOid oid);

        /// <summary>
        ///     Loads in data for an object for the specified identifier
        /// </summary>
        ObjectData LoadObjectData(IOid oid);

        void Remove(IOid oid);

        /// <summary>
        ///     Save the data for latter retrieval
        /// </summary>
        void Save(Data data);

        void Shutdown();

        /// <summary>
        ///     Return data for all instances that match the specification
        /// </summary>
        IQueryable<ObjectData> GetInstances(INakedObjectSpecification specification);

        Data LoadData(IOid oid);
    }

    // Copyright (c) Naked Objects Group Ltd.
}