// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
namespace NakedObjects.XmlStore {
    public class TestClock : IClock {
        internal long time = 0;

        #region IClock Members

        public long Ticks {
            get {
                lock (this) {
                    time += 1;
                    return time;
                }
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}