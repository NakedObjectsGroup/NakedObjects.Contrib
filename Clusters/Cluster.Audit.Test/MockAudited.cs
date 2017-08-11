using System;
using System.Collections.Generic;
using System.Linq;

using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Audit.Test
{
    public class MockAudited : IDomainInterface
    {

        #region LifeCycle Methods
         #endregion
        public virtual int Id { get; set; }

        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Name);
            return t.ToString();
        }
      

        [Optionally]
        public virtual string Name { get; set; }

        public void DoSomething()
        {

        }
      
    }
}
