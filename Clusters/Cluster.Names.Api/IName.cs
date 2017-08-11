using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.Names.Api
{
    /// <summary>
    /// Role interface implemented by any object that represents an individual's name.
    /// Note that this need not necessarily be a persisted object.
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// Richard Pawson
        /// </summary>
        /// <returns></returns>
 
        string NormalName {get;}

        /// <summary>
        /// e.g. Dr. Richard W Pawson
        /// </summary>
        /// <returns></returns>
        string FormalName { get; }

        /// <summary>
        /// e.g. Pawson, Richard
        /// </summary>
        /// <returns></returns>
        string SortableName { get; }

        /// <summary>
        /// e.g. Dr. Pawson
        /// </summary>
        /// <returns></returns>
        string FormalSalutation { get; }

        /// <summary>
        /// e.g. Richard
        /// </summary>
        /// <returns></returns>
        string InformalSalutation { get; }
    }
}
