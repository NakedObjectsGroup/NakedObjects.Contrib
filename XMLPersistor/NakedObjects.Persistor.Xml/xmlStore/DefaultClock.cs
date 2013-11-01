// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.XmlStore {
    public class DefaultClock : IClock {
        #region IClock Members

        public virtual long Ticks {
            get { return DateTime.Now.Ticks; }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}