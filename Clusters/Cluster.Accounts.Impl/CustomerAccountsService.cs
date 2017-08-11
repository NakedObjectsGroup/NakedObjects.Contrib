using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cluster.Accounts.Api;
using NakedObjects;
using NakedObjects.Services;


namespace Cluster.Accounts.Impl
{
    [DisplayName("Customer Accounts")]
    public class CustomerAccountsService : ICustomerAccountsService
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        public AccountsService AccountsService { set; protected get; }

        #endregion

        #region Accounts
        public ICustomerAccount FindById(int id)
        {
            return Container.Instances<CustomerAccount>().Single(x => x.Id == id);
        }

        public ICustomerAccount FindAccountByNumber(string accountNumber)
        {
            return Container.Instances<CustomerAccount>().Where(x => x.Code.Contains(accountNumber)).SingleOrDefault();
        }

        #region CreateNewAccount
        public ICustomerAccount CreateNewAccount(
            ICustomerAccountHolder forHolder, 
            string currencyCode, 
            [Mask("N")] decimal openingBalance,
            [Mask("d")] DateTime asOf,
            [Optionally, Named("Customer's own description")] string customersOwnDescription)
        {
            string name = "Customer A/c for "+forHolder.Name;
            //Code is null because it is set when the customer account is persisted
            CustomerAccount ca = AccountsService.CreateNewAccount<CustomerAccount>(null, name, AccountTypes.Asset, currencyCode, openingBalance, asOf);
            ca.AccountHolder = forHolder;
            ca.Description = customersOwnDescription;
            return ca;
        }

        public string Default1CreateNewAccount()
      {
          return AccountsService.Default3CreateNewAccount();
      }

        public IList<string> Choices1CreateNewAccount()
        {
            return AccountsService.Choices3CreateNewAccount();
        }

        public DateTime Default3CreateNewAccount()
        {
            return AccountsService.Default5CreateNewAccount();
        }

        public string ValidateCreateNewAccount(
            ICustomerAccountHolder forHolder,
            string currencyCode, 
            decimal openingBalance,
            DateTime asOf, 
            string newAccountName)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(FindAccounts(forHolder, newAccountName).Count() > 0, "Holder already has Account matching this name");
            return rb.Reason;
        }
        #endregion

        public IQueryable<ICustomerAccount> FindAccounts(ICustomerAccountHolder forHolder, string matchingName)
        {
            int c = AllAccountsForHolder(forHolder).Count();
           return AllAccountsForHolder(forHolder).Where(x => x.Description.ToUpper().Contains(matchingName.ToUpper()));
        }


        public IQueryable<ICustomerAccount> AllAccounts(ICustomerAccountHolder forHolder)
        {
            return AllAccountsForHolder(forHolder);
        }

        private IQueryable<CustomerAccount> AllAccountsForHolder(ICustomerAccountHolder forHolder)
        {
            return PolymorphicNavigator.FindOwners<CustomerAccountAccountHolderLink, ICustomerAccountHolder, CustomerAccount>(forHolder);
        }

        #endregion

    }
}
