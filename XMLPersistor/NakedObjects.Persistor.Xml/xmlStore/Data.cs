// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.XmlStore {
    public abstract class Data {
        private readonly IOid oid;
        private readonly string type;
        private readonly IVersion version;

        protected Data(INakedObjectSpecification type, IOid oid, IVersion version) {
            this.type = type.FullName;
            this.oid = oid;
            this.version = version;
        }

        public virtual IOid Oid {
            get { return oid; }
        }

        public virtual IVersion Version {
            get { return version; }
        }

        public virtual string ClassName {
            get { return type; }
        }

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            if (obj is Data) {
                return ((Data) obj).type.Equals(type) && ((Data) obj).oid.Equals(oid);
            }

            return false;
        }

        public override int GetHashCode() {
            int h = 17;
            h = 37*h + type.GetHashCode();
            h = 37*h + oid.GetHashCode();
            return h;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}