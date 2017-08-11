using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NakedObjects;

namespace Cluster.Accounts.Impl
{
    [NotMapped, DisplayName("Transaction")]
    public class TransactionInAccountView : IViewModel, IAccountEntry
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion
        #region Life Cycle Methods
        // This region should contain any of the 'life cycle' convention methods (such as
        // Created(), Persisted() etc) called by the framework at specified stages in the lifecycle.


        #endregion

        public string DisablePropertyDefault()
        {
            return "Not editable here  -  follow Transaction link  to edit";
        }


        #region Underlying persistent objects
        [NakedObjectsIgnore]
        public virtual Account AccountViewedFrom { get; set; }

        [MemberOrder(1)]  //Displayed to allow user to go to transaction & modify it.
        public virtual Transaction Transaction { get; set; }
        #endregion

        [MemberOrder(10), Mask("d")]
        public virtual DateTime Date
        {
            get
            {
                return Transaction.Date;
            }
        }


        [MemberOrder(20)]
        public virtual string Description
        {
            get
            {
                return Transaction.Description;
            }
        }


        [MemberOrder(30), Mask("N")]
        public virtual Decimal? Debit
        {
            get
            {
                if (Transaction.DebitAccount == AccountViewedFrom)
                {
                    return Transaction.Amount;
                }
                else
                {
                    return null;
                }
            }
        }

        [MemberOrder(40), Mask("N")]
        public virtual Decimal? Credit
        {
            get
            {
                if (Transaction.CreditAccount == AccountViewedFrom)
                {
                    return Transaction.Amount;
                }
                else
                {
                    return null;
                }
            }
        }


        #region Implementation of IViewModel
        public string[] DeriveKeys()
        {
            return new string[] { AccountViewedFrom.Id.ToString(), Transaction.Id.ToString() };
        }

        public void PopulateUsingKeys(string[] keys)
        {
            int accountId = int.Parse(keys[0]);
            AccountViewedFrom = Container.Instances<Account>().Where(x => x.Id == accountId).Single();
            int transactionId = int.Parse(keys[1]);
            Transaction = Container.Instances<Transaction>().Where(x => x.Id == transactionId).Single();
        }
        #endregion

        internal void MarkAsReconciled()
        {
            Transaction.Reconciled = true;
        }
    }
}

