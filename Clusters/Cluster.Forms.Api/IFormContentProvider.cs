using System;
using System.Collections.Generic;
using System.Linq;

namespace Cluster.Forms.Api
{
    /// <summary>
    /// Role interface implemented by an object in another cluster that can provide content (or default values)
    /// for an eForm
    /// </summary>
    public interface IFormContentProvider
    {
        /// <summary>
        /// If the implementing object does not recognise the fieldType/Name combination then it should
        /// just return null, not throw an error.
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        void PopulateTextField(string fieldName, out string value, out string display);

        void PopulateListField(string fieldName, out string[] exportValues, out string[] displayValues, out string[] selected);
      
    }
}
