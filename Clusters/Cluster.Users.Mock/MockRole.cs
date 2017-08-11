using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cluster.Users.Mock;
using NakedObjects;

namespace Cluster.Users.Mock
{
    [Bounded]
    [Immutable(WhenTo.OncePersisted)]
    public class MockRole
    {
        #region Injected Services
        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service; 'injc' to add an injected Container

        #endregion
        #region Life Cycle Methods
        // This region should contain any of the 'life cycle' convention methods (such as
        // Created(), Persisted() etc) called by the framework at specified stages in the lifecycle.


        #endregion

        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Name);
            return t.ToString();
        }
      

        [Hidden]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        #region Users (collection)
        private ICollection<MockUser> _Users = new List<MockUser>();

        public virtual ICollection<MockUser> Users
        {
            get
            {
                return _Users;
            }
            set
            {
                _Users = value;
            }
        }
        #endregion


    }
}

