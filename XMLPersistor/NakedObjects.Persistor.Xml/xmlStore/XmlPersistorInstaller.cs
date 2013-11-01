// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Common.Logging;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.XmlFile;

namespace NakedObjects.XmlStore {
    public class XmlPersistorInstaller : AbstractObjectPersistorInstaller {
        private static readonly ILog Log;
        private XmlObjectStore objectStore;

        static XmlPersistorInstaller() {
            Log = LogManager.GetLogger(typeof (XmlPersistorInstaller));
        }

        public XmlPersistorInstaller(string directory) :
            this(directory, true) {}

        public XmlPersistorInstaller(string directory, bool defaultLocation) {
            XmlDirectory = Utils.Path(directory, defaultLocation);
        }

        public string XmlDirectory { set; get; }

        #region IObjectPersistorInstaller Members

        public override string Name {
            get { return "xml"; }
        }

        public override INakedObjectPersistor CreateObjectPersistor() {
            Log.Info("installing " + GetType().FullName);

            if (objectStore == null) {
                objectStore = new XmlObjectStore {
                    Clock = new DefaultClock(),
                    XmlDirectory = XmlDirectory
                };
            }

            var persistAlgorithm = new DefaultPersistAlgorithm();
            var persistor = new ObjectStorePersistor {
                ObjectStore = objectStore,
                PersistAlgorithm = persistAlgorithm,
                OidGenerator = new TimeBasedOidGenerator(),
                IdentityMap = new IdentityMapImpl {IdentityAdapterMap = identityAdapterMap, PocoAdapterMap = pocoAdapterMap}
            };
            return persistor;
        }

        #endregion

        public INakedObjectReflector SetupReflector(INakedObjectReflector reflector) {
            //do nothing
            return reflector;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}