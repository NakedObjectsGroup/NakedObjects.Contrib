using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Cluster.System.Api;
using NakedObjects;
using NakedObjects.Services;
using NakedObjects.Util;

namespace Cluster.Audit.Impl
{
     [Immutable(WhenTo.OncePersisted)]
    public class ObjectPersisted : ObjectUpdated
    {
        #region Injected Services
        #endregion

        #region LifeCycle methods
#endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("Create & Save:").Append(this.Object.GetType().GetProxiedType().Name);
            return t.ToString();
        }
               
    }
}
