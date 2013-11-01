// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations.Schema;

namespace NakedObjects.XmlStore {
    public class Person {
        public Name Name { get; set; }
    }


    [ComplexType]
    public class Name {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}