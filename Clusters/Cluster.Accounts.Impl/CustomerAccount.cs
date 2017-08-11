
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cluster.Accounts.Api;
using Cluster.System.Api;
using NakedObjects;
using NakedObjects.Services;
using NakedObjects.Util;

namespace Cluster.Accounts.Impl
{
    /// <summary>
    /// Account representing a Receivables (or pre-payment) account for a customer
    /// </summary>
    public class CustomerAccount : Account, ICustomerAccount
    {
        #region Injected Services
        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        public ICustomerAccountNumberCreator AccountNumberCreator { set; protected get; }

        #endregion

        #region LifeCycle methods
        public override void Persisting()
        {
            base.Persisting();
            AccountHolderLink = PolymorphicNavigator.NewTransientLink<CustomerAccountAccountHolderLink, ICustomerAccountHolder, CustomerAccount>(_AccountHolder, this);
            string number = AccountNumberCreator.GetAccountNumberForNewAccount(this);
            Code = number;
        }
        #endregion

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Name).Append(" -",Description);
            return t.ToString();
        }

        #region Properties
        public override string DisableName()
        {
            return "Uneditable";
        }

        [MemberOrder(15), DisplayName("Customer's own description")]
        public virtual string Description { get; set; }
      
        #region AccountHolder Property of type ICustomerAccountHolder ('role' interface)

        [Hidden]
        public virtual CustomerAccountAccountHolderLink AccountHolderLink { get; set; }

        private ICustomerAccountHolder _AccountHolder;

        [NotPersisted, NotMapped, Disabled, MemberOrder(14)]
        public ICustomerAccountHolder AccountHolder
        {
            get
            {
                return PolymorphicNavigator.RoleObjectFromLink(ref _AccountHolder, AccountHolderLink, this);
            }
            set
            {
                _AccountHolder = value;
                AccountHolderLink = PolymorphicNavigator.UpdateAddOrDeleteLink(_AccountHolder, AccountHolderLink, this);
            }
        }
        #endregion

        [MemberOrder(50)]
        public virtual ClearanceMechanisms ClearanceMechanism { get; set; }

        #endregion

        #region Clearance 

        //public void Clear(Transaction credit, IEnumerable<Transaction> debits)
        //{
        //    decimal unclearedCredit = credit.UnclearedAmount();
        //    foreach (Transaction debit in debits)
        //    {
        //        var match = Container.NewTransientInstance<Match>();
        //    }
        //}

        //TODO:  Choices:  uncleared transactions of each type
        #endregion
    }

    public enum ClearanceMechanisms
    {
        ClearOldestDebitsFirst,
        ManualClearanceOnly
    }
}
