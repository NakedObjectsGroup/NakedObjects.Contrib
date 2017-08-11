using System.Collections.Generic;
using Cluster.Addresses.Api;
using NakedObjects;

namespace Cluster.Addresses.Impl
{
    [Immutable(WhenTo.OncePersisted)]
    public class GenericAddress : AbstractAddress
    {
        #region Injected Services

        #endregion

        #region Properties

        [MemberOrder(101)]
        public override string Line1 { get; set; }

        [MemberOrder(102)]
        public override string Line2 { get; set; }

        [MemberOrder(103), Optionally]
        public override string Line3 { get; set; }

        [MemberOrder(104), Optionally]
        public override string Line4 { get; set; }

        [MemberOrder(105), Optionally]
        public override string Line5 { get; set; }
        #endregion
    }
}
