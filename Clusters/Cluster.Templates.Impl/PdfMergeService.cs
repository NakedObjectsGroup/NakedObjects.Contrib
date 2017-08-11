using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using NakedObjects;
using iTextSharp.text.pdf;
using NakedObjects.Value;
using Cluster.Templates.Api;

namespace Cluster.Templates.Impl
{
    public class PdfMergeService
    {
        public byte[] MergeFieldsInPdfFile(byte[] inputPdf, IMergeFieldsProvider sourceObject)
        {
            var pr = new PdfReader(inputPdf);
            var ims = new MemoryStream();

            var ps = new PdfStamper(pr, ims);

            AcroFields af = ps.AcroFields;

            foreach (string fieldName in af.Fields.Keys)
            {
                try
                {
                    string content = sourceObject.GetContentsFor(fieldName);
                    af.SetField(fieldName, content);
                }
                catch
                {
                    //Field not found.
                }
            }

            ps.Close();

            byte[] output = ims.ToArray();

            return output;
        }


        //public byte[] TestFDFPrePopulated(byte[] pdf)
        //{
        //    var pr = new PdfReader(pdf);
        //    var ims = new MemoryStream();

        //    var ps = new PdfStamper(pr, ims);

        //    AcroFields af = ps.AcroFields;

        //    foreach (var field in af.Fields)
        //    {
        //        var x = field;
        //    }

        //    af.SetField("Text1", "Foo");

        //    af.SetField("Text2", "Bar");

        //    ps.Close();

        //    byte[] output = ims.ToArray();

        //    return output;
        //}


    }
}
