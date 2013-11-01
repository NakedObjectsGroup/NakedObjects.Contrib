// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.XmlFile {
    public class Utils {
        public static string Path(string directory, bool defaultLocation) {
            if (!System.IO.Path.IsPathRooted(directory) && defaultLocation) {
                string applicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return applicationData + @"\NakedObjects\" + directory;
            }
            return directory;
        }
    }
}