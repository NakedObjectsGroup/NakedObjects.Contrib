using System;
using System.Linq;
using System.Reflection;
using Microsoft.Office.Interop.Word;

using Word = Microsoft.Office.Interop.Word;

namespace Model.Communications
{
    public class WordMailMergeService :
    {


        private static string GetValueForMergeField(IDocumentMergeFieldsProvider sourceObject, string fieldName) {

            try {
                return sourceObject.GetContentsFor(fieldName);
            }

            catch {
                // log no such property
            }

            return "";
        }


       public void MergeFieldsInWordDoc(string sourcePath, IDocumentMergeFieldsProvider sourceObject, string destinationPath)
   {
            object missing = Missing.Value;

            // Hide MS Word's window.
            Application app = new Application { Visible = false };

            var path = sourcePath as object;
            _Document doc = app.Documents.Open(ref  path);


            MailMerge mailMerge = doc.MailMerge;

            foreach (MailMergeField f in mailMerge.Fields)
            {
                
                // Assuming the field code is: MERGEFIELD  "mailMergeFieldName"

                string fieldName = f.Code.Text;

                string newValue = GetValueForMergeField(sourceObject, fieldName);

                if (!string.IsNullOrWhiteSpace(newValue))
                {
                    f.Select();

                    // Replace selected field with supplied value.

                    app.Selection.TypeText(newValue);
                }
            }

            // Save changes and close MS Word.

            object newName = destinationPath as object;

            doc.SaveAs2(ref newName);

            object saveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;

            doc.Close(ref saveChanges, ref missing, ref missing);

            //TODO: Fix  -  check with Stef
            //app.Quit(ref missing, ref missing, ref missing);
        }
    }
}
