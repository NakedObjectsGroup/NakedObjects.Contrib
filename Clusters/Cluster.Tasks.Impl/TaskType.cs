using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NakedObjects;

namespace Cluster.Tasks.Impl
{
    /// <summary>
    /// Adopts the 'TypeAs Factory' pattern
    /// </summary>
    [Bounded, Immutable]
    public class TaskType
    {
        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Name);
            return t.ToString();
        }
      
        [Hidden]
        public virtual int Id { get; set; }

        [MemberOrder(10)]
        public virtual string Name { get; set; }

        [MemberOrder(20)]
        public virtual string Description { get; set; }

        /// <summary>
        /// Fully-qualified class name of type to create
        /// </summary>
        [Hidden]
        public virtual string CorrespondingClass { get; set; }
       
        public virtual bool UserCreatable { get; set; }
           
    }
}
