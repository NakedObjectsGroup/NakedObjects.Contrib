// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.Key;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Persistor.Transaction;

namespace NakedObjects.XmlStore {
    public class XmlObjectStore : INakedObjectStore {
        private static readonly ILog Log;
        private IDataManager dataManager;
        private IServiceManager serviceManager;

        static XmlObjectStore() {
            Log = LogManager.GetLogger(typeof (XmlObjectStore));
        }

        public XmlObjectStore() {}

        public XmlObjectStore(IDataManager dataManager, IServiceManager serviceManager, string xmlDirectory) {
            XmlDirectory = xmlDirectory;
            XmlFile.DirectoryName = XmlDirectory;
            this.dataManager = dataManager;
            this.serviceManager = serviceManager;
            serviceManager.LoadServices();
            IsInitialized = dataManager.IsInitialized;
        }

        public string XmlDirectory { get; set; }

        /// <summary>
        ///     Set the clock used to generate sequence numbers and last changed dates for version objects
        /// </summary>
        public virtual IClock Clock {
            set { FileVersion.Clock = value; }
        }

        private static INakedObjectPersistor Persistor {
            get { return NakedObjectsContext.ObjectPersistor; }
        }

        #region INakedObjectStore Members

        public bool IsInitialized { get; set; }

        public virtual void AbortTransaction() {
            Log.Debug("AbortTransaction");
        }

        public virtual ICreateObjectCommand CreateCreateObjectCommand(INakedObject nakedObject) {
            Log.DebugFormat("CreateCreateObjectCommand : {0}", nakedObject);
            return new CreateCommand(nakedObject, this);
        }

        public virtual void RegisterService(string name, IOid oid) {
            Log.DebugFormat("RegisterService name: {0} oid: {1}", name, oid);
            serviceManager.RegisterService(name, oid);
        }

        public virtual IDestroyObjectCommand CreateDestroyObjectCommand(INakedObject nakedObject) {
            Log.DebugFormat("CreateDestroyObjectCommand : {0}", nakedObject);
            return new DestroyCommand(nakedObject, this);
        }

        public virtual ISaveObjectCommand CreateSaveObjectCommand(INakedObject nakedObject) {
            Log.DebugFormat("CreateSaveObjectCommand : {0}", nakedObject);
            return new SaveCommand(nakedObject, this);
        }

        public virtual void EndTransaction() {
            Log.Debug("EndTransaction");
        }

        public virtual IQueryable<T> GetInstances<T>() where T : class {
            Log.DebugFormat("GetInstances<T> for: {0}", typeof (T));
            return from data in dataManager.GetInstances(NakedObjectsContext.Reflector.LoadSpecification(typeof (T)))
                   select CreateAndInitNakedObject(data).GetDomainObject<T>();
        }

        public virtual IQueryable GetInstances(Type type) {
            Log.DebugFormat("GetInstances for: {0}", type);
            return GetInstances(NakedObjectsContext.Reflector.LoadSpecification(type));
        }

        public virtual IQueryable GetInstances(INakedObjectSpecification specification) {
            Log.DebugFormat("GetInstances for: {0}", specification);
            return from data in dataManager.GetInstances(specification)
                   select CreateAndInitNakedObject(data).GetDomainObject();
        }

        public T CreateInstance<T>() where T : class {
            Log.DebugFormat("CreateInstance<T> for: {0}", typeof (T));
            return (T) CreateInstance(typeof (T));
        }

        public object CreateInstance(Type type) {
            Log.DebugFormat("CreateInstance for: {0}", type);
            return NakedObjectsContext.Reflector.LoadSpecification(type).CreateObject();
        }

        public virtual INakedObject GetObject(IOid oid, INakedObjectSpecification hint) {
            Log.DebugFormat("GetObject oid: {0} hint: {1}", oid, hint);
            Data data = dataManager.LoadData(oid);
            Log.DebugFormat("Data read {0}", data);

            if (data is ObjectData) {
                return RecreateObject((ObjectData) data);
            }
            if (data is CollectionData) {
                throw new InvalidDataException("Not expecting collection data: " + data);
            }

            throw new FindObjectException(oid);
        }

        public void Reload(INakedObject nakedObject) {
            Log.Debug("Reload");
            GetObject(nakedObject.Oid, nakedObject.Specification);
        }

        public virtual IOid GetOidForService(string name, string typeName) {
            Log.DebugFormat("GetOidForService name: {0}", name);
            return serviceManager.GetOidForService(name, typeName);
        }

        public virtual void Init() {
            Log.Debug("Init");
            XmlFile.DirectoryName = XmlDirectory; // do first - used by LoadServices
            dataManager = new XmlDataManager();
            serviceManager = new XmlServiceManager();
            serviceManager.LoadServices();
            IsInitialized = dataManager.IsInitialized;
        }

        public virtual string Name {
            get { return "XML"; }
        }

        /*
        * The ObjectData holds all references for internal collections, so the object should haves its internal
        * collection populated by this method.
        */

        public virtual void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {
            Log.DebugFormat("ResolveField nakedobject: {0} field: {1}", nakedObject, field);
            field.GetNakedObject(nakedObject);
            ResolveImmediately(nakedObject);
        }

        public virtual void ResolveImmediately(INakedObject nakedObject) {
            Log.DebugFormat("ResolveImmediately nakedobject: {0}", nakedObject);
            var data = (ObjectData) dataManager.LoadData(nakedObject.Oid);
            Assert.AssertNotNull("Not able to read in data during resolve", nakedObject, data);
            InitObject(nakedObject, data);
        }

        public void Reset() {
            Log.Debug("Reset");
        }

        public PropertyInfo[] GetKeys(Type type) {
            Log.Debug("GetKeys of: " + type);
            INakedObjectAssociation[] assocs = NakedObjectsContext.Reflector.LoadSpecification(type).Properties.Where(p => p.ContainsFacet<IKeyFacet>()).ToArray();
            return type.GetProperties().Where(p => assocs.Any(a => a.Id == p.Name)).ToArray();
        }

        public void Refresh(INakedObject nakedObject) {
            Log.DebugFormat("Refresh: {0}", nakedObject);
            throw new NotImplementedException();
        }

        public int CountField(INakedObject nakedObject, INakedObjectAssociation association) {
            return association.GetNakedObject(nakedObject).GetAsEnumerable().Count();
        }

        public INakedObject FindByKeys(Type type, object[] keys) {
            PropertyInfo[] keyProperties = GetKeys(type);

            if (keyProperties.Count() == keys.Count()) {
                IEnumerable<Tuple<PropertyInfo, object>> zip = keyProperties.Zip(keys, (pi, o) => new Tuple<PropertyInfo, object>(pi, o));
                IQueryable<object> objs = GetInstances(type).Cast<object>();
                object match = objs.SingleOrDefault(o => zip.All(z => z.Item1.GetValue(o, null).Equals(z.Item2)));
                return match == null ? null : NakedObjectsContext.ObjectPersistor.GetAdapterFor(match);
            }
            return null;
        }

        public virtual void Execute(IPersistenceCommand[] commands) {
            Log.DebugFormat("Execute {0} commands", commands.Length);
            foreach (IPersistenceCommand command in commands) {
                command.Execute(null);
            }
            Log.Debug("End execution");
        }

        public virtual bool Flush(IPersistenceCommand[] commands) {
            Log.DebugFormat("Flush {0} commands", commands.Length);
            foreach (IPersistenceCommand command in commands) {
                command.Execute(null);
            }
            Log.Debug("End flush");
            return commands.Length > 0;
        }

        public virtual void Shutdown() {
            Log.Debug("Shutdown");
        }

        public virtual void StartTransaction() {
            Log.Debug("StartTransaction");
        }

        #endregion

        private INakedObject CreateAndInitNakedObject(ObjectData instanceData) {
            Log.DebugFormat("Instance data {0}", instanceData);

            IOid oid = instanceData.Oid;
            INakedObjectSpecification spec = SpecFor(instanceData);
            INakedObject instance = PersistorUtils.RecreateInstance(oid, spec);
            InitObject(instance, instanceData);
            return instance;
        }

        private static INakedObjectSpecification SpecFor(Data data) {
            return NakedObjectsContext.Reflector.LoadSpecification(data.ClassName);
        }

        private static ObjectData CreateObjectData(INakedObject nakedObject, bool ensurePersistent) {
            Log.DebugFormat("Compiling object data for {0}", nakedObject);

            var data = new ObjectData(nakedObject.Specification, nakedObject.Oid, nakedObject.Version);

            foreach (INakedObjectAssociation fieldAssoc in nakedObject.Specification.Properties) {
                if (!fieldAssoc.IsPersisted) {
                    continue;
                }

                INakedObject field = fieldAssoc.GetNakedObject(nakedObject);

                string fieldId = fieldAssoc.Id;

                if (fieldAssoc.IsCollection) {
                    data.AddInternalCollection(field, fieldId, ensurePersistent);
                }
                if (fieldAssoc.IsInline) {
                    data.AddInline(CreateObjectData(field, ensurePersistent), fieldId);
                }
                else if (fieldAssoc.Specification.IsEncodeable) {
                    bool isEmpty = fieldAssoc.IsEmpty(nakedObject);
                    if (field == null || isEmpty) {
                        data.SaveValue(fieldId, isEmpty, null);
                    }
                    else {
                        var facet = field.Specification.GetFacet<IEncodeableFacet>();
                        string encodedValue = facet.ToEncodedString(field);
                        data.SaveValue(fieldId, isEmpty, encodedValue);
                    }
                }
                else if (fieldAssoc.IsObject) {
                    data.AddAssociation(field, fieldId, ensurePersistent);
                }
            }

            return data;
        }

        private void InitObject(INakedObject nakedObject, ObjectData data) {
            if (nakedObject.ResolveState.IsResolvable()) {
                nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
                SetupObject(nakedObject, data);
                nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
            }
        }

        private void SetupObject(INakedObject nakedObject, ObjectData data) {
            foreach (INakedObjectAssociation field in nakedObject.Specification.Properties.Where(field => field.IsPersisted)) {
                INakedObjectSpecification fieldSpecification = field.Specification;
                if (fieldSpecification.IsEncodeable) {
                    InitEncodeable(nakedObject, data, field, fieldSpecification);
                }
                else if (field.IsCollection) {
                    InitObjectSetupCollection(nakedObject, data, field);
                }
                else if (field.IsInline) {
                    InitInlineObjectSetupReference(nakedObject, data, field);
                }
                else if (field.IsObject) {
                    InitObjectSetupReference(nakedObject, data, field);
                }
            }
            nakedObject.OptimisticLock = data.Version;
        }

        private static void InitEncodeable(INakedObject nakedObject, ObjectData data, INakedObjectAssociation field, INakedObjectSpecification fieldSpecification) {
            INakedObject nakedObjectValue;
            string valueData = data.Value(field.Id);
            if (valueData == null || valueData.Equals("NULL")) {
                nakedObjectValue = null;
            }
            else {
                nakedObjectValue = fieldSpecification.GetFacet<IEncodeableFacet>().FromEncodedString(valueData);
            }
            ((IOneToOneAssociation) field).InitAssociation(nakedObject, nakedObjectValue);
        }

        private static void InitInlineObjectSetupReference(INakedObject nakedObject, ObjectData data, INakedObjectAssociation field) {
            var fieldData = (Data) data.GetField(field.Id);
            IOid referenceOid = fieldData.Oid;
            INakedObject reference = SetupReference(field, referenceOid, fieldData, nakedObject);
            Persistor.InitInlineObject(nakedObject.Object, reference.Object);
        }


        private void InitObjectSetupReference(INakedObject nakedObject, ObjectData data, INakedObjectAssociation field) {
            var referenceOid = (IOid) data.GetField(field.Id);
            if (referenceOid != null) {
                Data fieldData = dataManager.LoadData(referenceOid);
                INakedObject reference = SetupReference(field, referenceOid, fieldData, nakedObject);
            }
        }

        private static INakedObject SetupReference(INakedObjectAssociation field, IOid referenceOid, Data fieldData, INakedObject nakedObject) {
            Log.DebugFormat("Setting up field {0} with {1}", field, referenceOid);
            INakedObject reference = null;
            if (fieldData == null) {
                INakedObject adapter = PersistorUtils.RecreateInstance(referenceOid, field.Specification);
                if (!adapter.ResolveState.IsDestroyed()) {
                    nakedObject.ResolveState.Handle(Events.DestroyEvent);
                }
                ((IOneToOneAssociation) field).InitAssociation(nakedObject, adapter);

                Log.Warn("No data found for " + referenceOid + " so field '" + field.Name + "' not set in object '" + nakedObject + "'");
            }
            else {
                reference = PersistorUtils.RecreateInstance(referenceOid, SpecFor(fieldData));
                ((IOneToOneAssociation) field).InitAssociation(nakedObject, reference);
            }
            return reference;
        }

        private void InitObjectSetupCollection(INakedObject nakedObject, ObjectData data, INakedObjectAssociation field) {
            /*
            * The internal collection is already a part of the object, and therefore cannot be recreated, but its
            * oid must be set
            */
            IList<IOid> refs = (IList<IOid>) data.GetField(field.Id) ?? new List<IOid>();
            INakedObject collection = field.GetNakedObject(nakedObject);
            collection.ResolveState.Handle(Events.StartResolvingEvent);

            var elements = new List<INakedObject>();
            foreach (IOid elementOid in refs) {
                INakedObject adapter = Persistor.GetAdapterFor(elementOid) ?? GetObject(elementOid, null);
                elements.Add(adapter);
            }
            ICollectionFacet facet = collection.GetCollectionFacetFromSpec();
            facet.Init(collection, elements.ToArray());
            collection.ResolveState.Handle(Events.EndResolvingEvent);
        }

        private INakedObject RecreateObject(ObjectData data) {
            INakedObject nakedObject = PersistorUtils.RecreateInstance(data.Oid, SpecFor(data));
            InitObject(nakedObject, data);
            return nakedObject;
        }

        #region Nested Type: CreateCommand

        private class CreateCommand : ICreateObjectCommand {
            private readonly INakedObject nakedObject;
            private readonly XmlObjectStore xmlStore;

            public CreateCommand(INakedObject nakedObject, XmlObjectStore xmlStore) {
                this.nakedObject = nakedObject;
                this.xmlStore = xmlStore;
            }

            #region ICreateObjectCommand Members

            public virtual void Execute(IExecutionContext context) {
                Log.DebugFormat("Create object {0}", nakedObject);
                string user = NakedObjectsContext.Session.UserName;
                nakedObject.OptimisticLock = new FileVersion(user);
                ObjectData data = CreateObjectData(nakedObject, true);
                xmlStore.dataManager.InsertObject(data);
                SafePersisted();
            }

            public virtual INakedObject OnObject() {
                return nakedObject;
            }

            private void SafePersisted() {
                if (nakedObject != null && nakedObject.Specification != null && nakedObject.Specification.ContainsFacet<IPersistedCallbackFacet>()) {
                    nakedObject.Persisted();
                }
            }

            #endregion

            public override string ToString() {
                return "CreateObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion

        #region Nested Type: DestroyCommand

        private class DestroyCommand : IDestroyObjectCommand {
            private readonly INakedObject nakedObject;
            private readonly XmlObjectStore xmlStore;

            public DestroyCommand(INakedObject nakedObject, XmlObjectStore xmlStore) {
                this.nakedObject = nakedObject;
                this.xmlStore = xmlStore;
            }

            #region IDestroyObjectCommand Members

            public virtual void Execute(IExecutionContext context) {
                Log.DebugFormat("Destroy object {0}", nakedObject);
                IOid oid = nakedObject.Oid;
                xmlStore.dataManager.Remove(oid);
                nakedObject.OptimisticLock = new NullVersion();
            }

            public virtual INakedObject OnObject() {
                return nakedObject;
            }

            #endregion

            public override string ToString() {
                return "DestroyObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion

        #region Nested Type: SaveCommand

        private class SaveCommand : ISaveObjectCommand {
            private readonly INakedObject nakedObject;
            private readonly XmlObjectStore xmlStore;

            public SaveCommand(INakedObject nakedObject, XmlObjectStore xmlStore) {
                this.nakedObject = nakedObject;
                this.xmlStore = xmlStore;
            }

            #region ISaveObjectCommand Members

            public virtual void Execute(IExecutionContext context) {
                Log.DebugFormat("Save object {0}", nakedObject);
                string user = NakedObjectsContext.Session.UserName;
                nakedObject.OptimisticLock = new FileVersion(user);

                Data data = CreateObjectData(nakedObject, true);
                xmlStore.dataManager.Save(data);
            }

            public virtual INakedObject OnObject() {
                return nakedObject;
            }

            #endregion

            public override string ToString() {
                return "SaveObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}