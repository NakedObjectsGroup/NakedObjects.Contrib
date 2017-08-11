using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Forms.Api;
using NakedObjects;

namespace Cluster.Forms.Impl
{
    [Immutable]
    public class FormSubmissionField
    {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(10)]
        public virtual FieldTypes Type { get; set; }

        [MemberOrder(20)]
        public virtual string Label { get; set; }

        [MemberOrder(30)]
        public virtual string Value { get; set; }
    }
}
