using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Accounts.Api;
using NakedObjects;

namespace Cluster.Accounts.Test
{
    public class MockAccountHolder : ICustomerAccountHolder
    {
        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Name);
            return t.ToString();
        }
      
        public virtual int Id { get; set; }

        
        public virtual string Name { get; set; }     
    }
}
