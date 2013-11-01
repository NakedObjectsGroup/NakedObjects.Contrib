// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.XmlStore {
    /// <summary>
    ///     A logical collection of elements of a specified Type
    /// </summary>
    public class CollectionData : Data {
        private readonly IList<IOid> elements = new List<IOid>();

        public CollectionData(INakedObjectSpecification type, IOid oid, IVersion version)
            : base(type, oid, version) {}

        public virtual void AddElement(IOid elementOid) {
            elements.Add(elementOid);
        }

        public virtual void RemoveElement(IOid elementOid) {
            elements.Remove(elementOid);
        }

        public virtual IList<IOid> References() {
            return elements;
        }

        public override string ToString() {
            return "CollectionData[Type=" + ClassName + ",elements=" + elements + "]";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}