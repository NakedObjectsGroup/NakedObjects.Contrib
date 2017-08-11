using System;
using System.Collections.Generic;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using Cluster.Templates.Api;

namespace Cluster.Templates.Impl
{
    public class WordMailMergeService
    {

        public static void ReplaceMailMergeFields(string dir, string msWordFileName, IMergeFieldsProvider domainObject, string fileSuffix)
        {
            object docName = msWordFileName;

            object missing = Missing.Value;

            // Hide MS Word's window.
            var app = new Application { Visible = false } as _Application;

            var path = dir + docName as object;

            _Document doc = app.Documents.Open(ref path);

            MailMerge mailMerge = doc.MailMerge;

            // Try to find the field name.

            foreach (MailMergeField f in mailMerge.Fields)
            {
                // Assuming the field code is: MERGEFIELD  "mailMergeFieldName"
                string mailMergeFieldName = f.Code.Text;

                string newValue = domainObject.GetContentsFor(mailMergeFieldName);

                if (!string.IsNullOrWhiteSpace(newValue))
                {
                    f.Select();

                    // Replace selected field with supplied value.

                    app.Selection.TypeText(newValue);
                }
            }

            // Save changes and close MS Word.

            object newName =dir + msWordFileName.Replace(".docx", "") + "-" + fileSuffix + ".docx";

            doc.SaveAs2(ref newName);

            object saveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;

            doc.Close(ref saveChanges, ref missing, ref missing);

            app.Quit(ref missing, ref missing, ref missing);
        }


    }
}
