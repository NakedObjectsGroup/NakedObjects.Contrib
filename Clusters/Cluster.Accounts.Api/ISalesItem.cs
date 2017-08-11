using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cluster.System.Api;
using NakedObjects;

namespace Cluster.Accounts.Api
{
    /// <summary>
    /// Role interface implemented by any object that can be deemed to be a sales transaction.  The object must supply details
    /// such as amount, description.
    /// If there is tax, discounts, etc, then these may be posted as separate items.
    /// </summary>
    public interface ISalesItem : IDomainInterface
    {
        string CurrencyCode();

        decimal Amount { get; }

        string Description { get; }

        string AnalysisCodes { get; }

        /// <summary>
        /// Call-back method that  - for example, allows the implementing item to be released/shipped/executed.
        /// </summary>
        void NotifyWhenPaid();
    }

}
