// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.IO;
using System.Text;
using System.Xml;

namespace NakedObjects.XmlStore {
    public static class XmlFile {
        private const string defaultEncoding = "ISO-8859-1";
        private static readonly string charset;
        private static DirectoryInfo directoryInfo;
        private static string directoryName;

        static XmlFile() {
            charset = defaultEncoding;
        }

        /// <summary>
        ///     The XML store is deemed to be uninitialised if the directoryInfo used to store the data has no xml files in
        ///     it.
        /// </summary>
        public static bool IsInitialized {
            get { return directoryInfo.GetFiles("*.xml").Length > 0; }
        }

        public static string DirectoryName {
            get { return directoryName; }
            set {
                directoryName = value;
                SetupDirectory();
            }
        }

        public static DirectoryInfo DirectoryInfo {
            get { return directoryInfo; }
        }


        public static Encoding Encoding {
            get { return Encoding.GetEncoding(charset); }
        }

        public static XmlWriterSettings Settings {
            get {
                var settings = new XmlWriterSettings {Indent = true, Encoding = Encoding};
                return settings;
            }
        }

        private static void SetupDirectory() {
            directoryInfo = new DirectoryInfo(DirectoryName);
            bool exists = File.Exists(directoryInfo.FullName) || Directory.Exists(directoryInfo.FullName);
            if (!exists) {
                Directory.CreateDirectory(directoryInfo.FullName);
            }
        }

        public static FileInfo GetFile(string fileName) {
            return new FileInfo(directoryInfo.FullName + "\\" + fileName + ".xml");
        }

        public static void Delete(string fileName) {
            if (File.Exists(GetFile(fileName).FullName)) {
                File.Delete(GetFile(fileName).FullName);
            }
            else if (Directory.Exists(GetFile(fileName).FullName)) {
                Directory.Delete(GetFile(fileName).FullName);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}