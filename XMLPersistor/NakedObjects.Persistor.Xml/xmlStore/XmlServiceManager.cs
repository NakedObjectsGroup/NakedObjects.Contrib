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
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;

namespace NakedObjects.XmlStore {
    internal class ServiceElement {
        private readonly string id;
        private readonly SerialOid oid;
        private readonly string typeName;

        public ServiceElement(SerialOid oid, string id) {
            Assert.AssertNotNull("oid", oid);
            Assert.AssertNotNull("id", id);
            this.oid = oid;
            this.id = id;
            typeName = oid.Specification.FullName;
            oid.Specification.MarkAsService();
        }

        internal string Id {
            get { return id; }
        }

        internal SerialOid Oid {
            get { return oid; }
        }

        internal string TypeName {
            get { return typeName; }
        }
    }


    public class XmlServiceManager : IServiceManager {
        private const string SERVICES_FILE_NAME = "services";
        private IList<ServiceElement> services;

        #region IServiceManager Members

        public virtual IOid GetOidForService(string name, string typeName) {
            foreach (ServiceElement element in services) {
                if (element.Id.Equals(name)) {
                    return element.Oid;
                }
            }
            SerialOid oid = SerialOid.CreatePersistent(services.Count(), typeName);
            RegisterService(name, oid);
            return oid;
        }

        public virtual void LoadServices() {
            services = new List<ServiceElement>();
            FileInfo file = XmlFile.GetFile(SERVICES_FILE_NAME);

            if (file.Exists) {
                XDocument doc = XDocument.Load(file.FullName);

                foreach (XElement element in doc.Element("services").Elements("service")) {
                    string sOid = (from attrib in element.Attributes()
                                   where attrib.Name == "oid"
                                   select attrib.Value).Single();

                    long oid = Convert.ToInt64(sOid, 16);

                    string id = (from attrib in element.Attributes()
                                 where attrib.Name == "id"
                                 select attrib.Value).Single();

                    string type = (from attrib in element.Attributes()
                                   where attrib.Name == "type"
                                   select attrib.Value).SingleOrDefault();

                    type = type ?? id;

                    var service = new ServiceElement(SerialOid.CreatePersistent(oid, type), id);
                    services.Add(service);
                }
            }
        }


        public virtual void RegisterService(string name, IOid oid) {
            services.Add(new ServiceElement((SerialOid) oid, name));
            SaveServices();
        }

        #endregion

        private static string EncodedOid(SerialOid oid) {
            return oid.SerialNo.ToString("x").ToUpper();
        }

        public void SaveServices() {
            using (XmlWriter writer = XmlWriter.Create(XmlFile.GetFile(SERVICES_FILE_NAME).FullName, XmlFile.Settings)) {
                writer.WriteStartElement("services");
                foreach (ServiceElement element in services) {
                    writer.WriteStartElement("service");
                    writer.WriteAttributeString("oid", EncodedOid(element.Oid));
                    writer.WriteAttributeString("id", element.Id);
                    writer.WriteAttributeString("type", element.TypeName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}