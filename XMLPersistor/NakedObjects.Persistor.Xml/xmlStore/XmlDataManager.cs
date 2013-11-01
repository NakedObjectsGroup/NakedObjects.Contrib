// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.XmlStore {
    public class XmlDataManager : IDataManager {
        private bool typesLoaded;

        #region IDataManager Members

        public virtual string DebugData {
            get { return "Data directory " + XmlFile.DirectoryName; }
        }

        public virtual bool IsInitialized {
            get { return XmlFile.IsInitialized; }
        }

        // TODO the following methods are being called repeatedly - is there no caching? See the print statements

        public IOid CreateOid(string typeName) {
            return SerialOid.CreatePersistent(NextId(), typeName);
        }

        public virtual IQueryable<ObjectData> GetInstances(INakedObjectSpecification specification) {
            EnsureTypesLoaded();
            return from instance in LoadInstances(specification)
                   select (ObjectData) LoadData(instance);
        }

        /// <summary>
        ///     Save the data for an object and adds the reference to a list of instances
        /// </summary>
        public void InsertObject(ObjectData data) {
            if (data.Oid == null) {
                throw new ArgumentException("Oid must be non-null");
            }
            string type = data.ClassName;
            IOid oid = data.Oid;
            AddData(oid, type, data);
            AddInstance(oid, type);
        }

        /// <summary>
        ///     Loads in data for a collection for the specified identifier
        /// </summary>
        public CollectionData LoadCollectionData(IOid oid) {
            return (CollectionData) LoadData(oid);
        }


        public virtual Data LoadData(IOid oid) {
            XDocument doc = XDocument.Load(XmlFile.GetFile(Filename(oid)).FullName);

            CollectionData collection = null;
            ObjectData objectData = null;
            string fieldName = "";

            foreach (XElement element in doc.Descendants()) {
                LoadNode(element, ref collection, ref objectData, ref fieldName);
            }

            if (objectData != null) {
                return objectData;
            }
            if (collection != null) {
                return collection;
            }
            throw new FindObjectException("No data found for " + oid + " (possible missing file)");
        }

        /// <summary>
        ///     Loads in data for an object for the specified identifier
        /// </summary>
        public ObjectData LoadObjectData(IOid oid) {
            return (ObjectData) LoadData(oid);
        }

        public void Remove(IOid oid) {
            Data data = LoadData(oid);
            string type = data.ClassName;
            RemoveInstance(oid, type);
            DeleteData(oid, type);
        }

        /// <summary>
        ///     Save the data for latter retrieval
        /// </summary>
        public void Save(Data data) {
            WriteData(data.Oid, data);
        }

        public virtual void Shutdown() {}

        #endregion

        public void LoadNode(XElement Node, ref CollectionData collection, ref ObjectData objectData, ref string fieldName) {
            string tag = Node.Name.LocalName;
            if (objectData != null) {
                if (tag.Equals("value")) {
                    fieldName = Node.Attribute("field").Value;
                    objectData.SetField(fieldName, Node.Value);
                }
                else if (tag.Equals("inline")) {
                    CollectionData sinkCollection = null;
                    ObjectData inlineObjectData = null;
                    string sinkName = "";
                    fieldName = Node.Attribute("field").Value;
                    LoadNode(Node.Element("naked-object"), ref sinkCollection, ref inlineObjectData, ref sinkName);
                    objectData.SetField(fieldName, inlineObjectData);
                }
                else if (tag.Equals("association")) {
                    fieldName = Node.Attribute("field").Value;
                    long id = Convert.ToInt64(Node.Attribute("ref").Value, 16);
                    objectData.SetField(fieldName, SerialOid.CreatePersistent(id, Node.Attribute("Type").Value));
                }
                else if (tag.Equals("element")) {
                    long id = Convert.ToInt64(Node.Attribute("ref").Value, 16);
                    objectData.AddElement(fieldName, SerialOid.CreatePersistent(id, Node.Attribute("Type").Value));
                }
                else if (tag.Equals("multiple-association")) {
                    fieldName = Node.Attribute("field").Value;
                    objectData.InitCollection(fieldName);
                }
            }
            else if (collection != null) {
                if (tag.Equals("element")) {
                    long id = Convert.ToInt64(Node.Attribute("ref").Value, 16);
                    collection.AddElement(SerialOid.CreatePersistent(id, Node.Attribute("Type").Value));
                }
            }
            else {
                if (tag.Equals("naked-object")) {
                    string type = Node.Attribute("Type").Value;
                    string user = Node.Attribute("user").Value;
                    IVersion version = GetVersion(Node, user);
                    IOid oid = GetOid(Node, type);
                    INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(type);

                    objectData = new ObjectData(spec, oid, version);
                }
                else if (tag.Equals("collection")) {
                    string type = Node.Attribute("Type").Value;
                    long version = Convert.ToInt64(Node.Attribute("ver").Value, 16);
                    string user = Node.Attribute("user").Value;
                    long id = Convert.ToInt64(Node.Attribute("id").Value, 16);
                    INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(type);
                    IOid oid = SerialOid.CreatePersistent(id, type);
                    collection = new CollectionData(spec, oid, new FileVersion(user, version));
                }
                else {
                    throw new XmlException("Invalid data");
                }
            }
        }

        private static IOid GetOid(XElement node, string typeName) {
            string[] values = node.Attribute("id").Value.Split(':');

            if (values.Count() <= 0 || values.Count() > 2) {
                return null;
            }

            long id = Convert.ToInt64(values[0], 16);
            IOid oid = SerialOid.CreatePersistent(id, typeName);

            if (values.Count() == 2) {
                oid = new AggregateOid(oid, values[1], typeName);
            }

            return oid;
        }

        private static IVersion GetVersion(XElement node, string user) {
            IVersion version = new NullVersion();
            if (node.Attribute("ver") != null) {
                long versionVal = Convert.ToInt64(node.Attribute("ver").Value, 16);
                version = new FileVersion(user, versionVal);
            }
            return version;
        }

        /// <summary>
        ///     Write out the data for a new instance
        /// </summary>
        protected internal virtual void AddData(IOid oid, string type, Data data) {
            WriteData(oid, data);
        }

        /// <summary>
        ///     Add the reference for an instance to the list of all instances
        /// </summary>
        protected internal virtual void AddInstance(IOid oid, string type) {
            IList<IOid> instances = LoadInstances(type).ToList();
            instances.Add(oid);
            WriteInstanceFile(type, instances);
        }

        /// <summary>
        ///     Delete the data for an existing instance
        /// </summary>
        protected internal virtual void DeleteData(IOid oid, string type) {
            XmlFile.Delete(Filename(oid));
        }

        private static string EncodedOid(IOid oid) {
            if (oid is SerialOid) {
                return (oid as SerialOid).SerialNo.ToString("x").ToUpper();
            }
            if (oid is AggregateOid) {
                var aOid = oid as AggregateOid;
                return EncodedOid(aOid.ParentOid) + ":" + aOid.FieldName;
            }
            return null;
        }

        private static string Filename(IOid oid) {
            return EncodedOid(oid);
        }

        private static IQueryable<IOid> LoadInstances(INakedObjectSpecification spec) {
            List<IOid> instances = LoadInstances(spec.FullName).ToList();
            foreach (INakedObjectSpecification subSpec in spec.Subclasses) {
                instances.AddRange(LoadInstances(subSpec).ToList());
            }
            return instances.AsQueryable();
        }


        private static IQueryable<IOid> LoadInstances(string type) {
            FileInfo typeFile = XmlFile.GetFile(type);
            if (typeFile.Exists) {
                return (from instance in XDocument.Load(typeFile.FullName).Element("instances").Elements("instance")
                        let sOid = (string) instance.Attribute("id")
                        select SerialOid.CreatePersistent(Convert.ToInt64(sOid, 16), type)).Cast<IOid>().AsQueryable();
            }
            return new List<IOid>().AsQueryable();
        }

        /// <summary>
        ///     Read in the next unique number for the object identifier
        /// </summary>
        public virtual long NextId() {
            string sId = null;
            if (XmlFile.GetFile("oid").Exists) {
                XDocument doc = XDocument.Load(XmlFile.GetFile("oid").FullName);
                sId = doc.Element("number").Value;
            }

            long longValue = string.IsNullOrEmpty(sId) ? 0 : Convert.ToInt64(sId, 16);

            long nextId = longValue + 1;
            using (XmlWriter writer = XmlWriter.Create(XmlFile.GetFile("oid").FullName, XmlFile.Settings)) {
                writer.WriteStartElement("number");
                writer.WriteValue(nextId.ToString("x"));
                writer.WriteEndElement();
            }

            return nextId;
        }

        /// <summary>
        ///     Remove the reference for an instance from the list of all instances
        /// </summary>
        protected internal virtual void RemoveInstance(IOid oid, string type) {
            IList<IOid> instances = LoadInstances(type).ToList();
            instances.Remove(oid);
            WriteInstanceFile(type, instances);
        }

        private static void WriteData(IOid xoid, Data data) {
            using (XmlWriter writer = XmlWriter.Create(XmlFile.GetFile(Filename(xoid)).FullName, XmlFile.Settings)) {
                WriteData(data, writer);
            }
        }

        private static void WriteData(Data data, XmlWriter writer) {
            bool isObject = data is ObjectData;
            string tag = isObject ? "naked-object" : "collection";

            writer.WriteStartElement(tag);
            writer.WriteAttributeString("Type", data.ClassName);
            writer.WriteAttributeString("id", EncodedOid(data.Oid));
            writer.WriteAttributeString("user", NakedObjectsContext.Session.UserName);

            string sequenceString = data.Version.AsSequence();
            if (!string.IsNullOrEmpty(sequenceString)) {
                long sequence = Convert.ToInt64(sequenceString, 16);
                writer.WriteAttributeString("ver", "" + Convert.ToString(sequence, 16).ToUpper());
            }
            if (isObject) {
                var objectData = (ObjectData) data;

                for (IEnumerator<string> fields = objectData.Fields(); fields.MoveNext();) {
                    string field = fields.Current;
                    object entry = objectData.GetField(field);

                    if (entry is IOid) {
                        var oid = entry as IOid;
                        Assert.AssertFalse(oid.IsTransient);
                        writer.WriteStartElement("association");
                        writer.WriteAttributeString("field", field);
                        writer.WriteAttributeString("ref", EncodedOid(oid));
                        writer.WriteAttributeString("Type", oid.Specification.FullName);
                        writer.WriteEndElement();
                    }
                    else if (entry is IList<IOid>) {
                        var references = (IList<IOid>) entry;

                        if (references.Count > 0) {
                            writer.WriteStartElement("multiple-association");
                            writer.WriteAttributeString("field", field);
                            foreach (SerialOid oid in references) {
                                Assert.AssertFalse("transient oid", oid, oid.IsTransient);
                                writer.WriteStartElement("element");
                                writer.WriteAttributeString("ref", EncodedOid(oid));
                                writer.WriteAttributeString("Type", oid.Specification.FullName);
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                    }
                    else if (entry is ObjectData) {
                        writer.WriteStartElement("inline");
                        writer.WriteAttributeString("field", field);
                        WriteData(entry as ObjectData, writer);
                        writer.WriteEndElement();
                    }
                    else {
                        writer.WriteStartElement("value");
                        writer.WriteAttributeString("field", field);
                        writer.WriteString(entry.ToString());

                        writer.WriteEndElement();
                    }
                }
            }
            else {
                var collection = (CollectionData) data;
                IList<IOid> refs = collection.References();
                foreach (IOid oid in refs) {
                    writer.WriteStartElement("element");
                    writer.WriteAttributeString("ref", EncodedOid(oid));
                    writer.WriteAttributeString("Type", oid.Specification.FullName);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private static void WriteInstanceFile(string name, IEnumerable<IOid> instances) {
            using (XmlWriter writer = XmlWriter.Create(XmlFile.GetFile(name).FullName, XmlFile.Settings)) {
                writer.WriteStartElement("instances");
                writer.WriteAttributeString("name", name);
                foreach (IOid instance in instances) {
                    writer.WriteStartElement("instance");
                    writer.WriteAttributeString("id", EncodedOid(instance));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void EnsureTypesLoaded() {
            // leave until last moment so that assemblies are in memory
            if (!typesLoaded) {
                LoadKnownTypes();
                typesLoaded = true;
            }
        }

        private static void LoadKnownTypes() {
            foreach (FileInfo file in new DirectoryInfo(XmlFile.DirectoryName).GetFiles("*.xml")) {
                string possibleTypeName = file.Name.Substring(0, file.Name.LastIndexOf(".xml"));
                Type possibleType = TypeUtils.GetType(possibleTypeName);
                if (possibleType != null) {
                    NakedObjectsContext.Reflector.LoadSpecification(possibleType);
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}