// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Text;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;

namespace NakedObjects.XmlStore {
    /// <summary>
    ///     A logical collection of elements of a specified Type
    /// </summary>
    public class ObjectData : Data {
        private readonly IDictionary<string, object> fields = new Dictionary<string, object>();

        public ObjectData(INakedObjectSpecification type, IOid oid, IVersion version)
            : base(type, oid, version) {}

        private string FieldsToString() {
            var sb = new StringBuilder();

            foreach (string s in fields.Keys) {
                sb.Append("[" + s + "=" + fields[s] + "]");
            }

            return sb.ToString();
        }

        public override string ToString() {
            return "ObjectData[Type=" + ClassName + ",oid=" + Oid + ",fields=" + FieldsToString() + "]";
        }

        public virtual void SetField(string fieldId, object obj) {
            lock (fields) {
                if (obj == null) {
                    fields.Remove(fieldId);
                }
                else {
                    fields[fieldId] = obj;
                }
            }
        }

        public virtual void SaveValue(string fieldId, bool isEmpty, string encodedString) {
            lock (fields) {
                if (isEmpty) {
                    fields.Remove(fieldId);
                }
                else {
                    fields[fieldId] = encodedString;
                }
            }
        }

        public virtual void SetField(string fieldId, string stringValue) {
            lock (fields) {
                fields[fieldId] = stringValue;
            }
        }

        public virtual object GetField(string fieldId) {
            lock (fields) {
                return fields.ContainsKey(fieldId) ? fields[fieldId] : null;
            }
        }

        public virtual string Value(string fieldId) {
            return (string) GetField(fieldId);
        }

        public virtual string GetId(string fieldId) {
            object field = GetField(fieldId);
            return field == null ? null : ((SerialOid) field).SerialNo.ToString();
        }

        public virtual void InitCollection(string fieldId) {
            lock (fields) {
                fields[fieldId] = new List<IOid>();
            }
        }

        public virtual void AddElement(string fieldId, IOid elementOid) {
            lock (fields) {
                if (!fields.ContainsKey(fieldId)) {
                    throw new InvalidDataException("Field " + fieldId + " not found  in dictionary");
                }

                var v = (IList<IOid>) fields[fieldId];
                v.Add(elementOid);
            }
        }

        public virtual IList<IOid> Elements(string fieldId) {
            lock (fields) {
                if (fields.ContainsKey(fieldId)) {
                    return (IList<IOid>) fields[fieldId];
                }
            }
            return null;
        }

        public virtual IEnumerator<string> Fields() {
            return fields.Keys.GetEnumerator();
        }

        public virtual void AddAssociation(INakedObject fieldContent, string fieldId, bool ensurePersistent) {
            bool notAlreadyPersistent = fieldContent != null && fieldContent.Oid.IsTransient;
            if (ensurePersistent && notAlreadyPersistent) {
                throw new Exception(string.Format("Cannot save an object that is not persistent: {0}", fieldContent));
            }
            SetField(fieldId, fieldContent == null ? null : fieldContent.Oid);
        }

        public virtual void AddInline(ObjectData fieldContent, string fieldId) {
            SetField(fieldId, fieldContent);
        }

        public virtual void AddInternalCollection(INakedObject collection, string fieldId, bool ensurePersistent) {
            InitCollection(fieldId);

            foreach (INakedObject element in collection.GetAsEnumerable()) {
                IOid elementOid = element.Oid;
                if (elementOid == null) {
                    throw new Exception("Element is not persistent " + element);
                }

                AddElement(fieldId, elementOid);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}