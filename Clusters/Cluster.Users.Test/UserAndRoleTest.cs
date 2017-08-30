using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using Helpers;
using Cluster.Users.Impl;

namespace Cluster.Users.Test
{
    [TestClass, Ignore]
    public class UserAndRoleTest : AbstractUsersTest
    {
        #region Run Configuration

        protected override IAuthorizerInstaller Authorizer
        {
            get
            {
                return new CustomAuthorizerInstaller(
                    new TestDefaultAuthorizer()
                    //new UsersAuthorizer()
                );
            }
        }
        #endregion

        [TestMethod, TestCategory("UserAndRoleTest")] //TODO: fix this
        public void UserPropertiesAndActions()
        {
            
            var rich = Users.GetAction("All Users").InvokeReturnCollection().ElementAt(0);

            //properties
            Assert.AreEqual(6, rich.Properties.Count()); // was 7

            var userName = rich.Properties.ElementAt(0).AssertIsVisible().AssertIsUnmodifiable();
            Assert.AreEqual("User Name", userName.Name);

            var fullName = rich.Properties.ElementAt(1).AssertIsVisible().AssertIsModifiable().AssertIsOptional();
            Assert.AreEqual("Full Name", fullName.Name);

            var email = rich.Properties.ElementAt(2).AssertIsVisible().AssertIsModifiable();
            Assert.AreEqual("Email Address", email.Name);

            var orgs = rich.Properties.ElementAt(3).AssertIsVisible();
            Assert.AreEqual("Organisations", orgs.Name);
            Assert.IsNotNull(orgs.ContentAsCollection);

            var last = rich.Properties.ElementAt(4).AssertIsVisible();
            Assert.AreEqual("Last Modified", last.Name);

            var roles = rich.Properties.ElementAt(5).AssertIsVisible();
            Assert.AreEqual("Roles", roles.Name);

            //Actions
            Assert.AreEqual(3, rich.Actions.Count()); 

            var addOrg = rich.Actions.ElementAt(0);
            Assert.AreEqual("Add Organisation", addOrg.Name);
            Assert.AreEqual(1, addOrg.Parameters.Count());
            Assert.AreEqual("Organisation", addOrg.Parameters[0].Name);

            var removeOrg = rich.Actions.ElementAt(1);
            Assert.AreEqual("Remove Organisation", removeOrg.Name);
            Assert.AreEqual(1, removeOrg.Parameters.Count());
            Assert.AreEqual("Organisation", removeOrg.Parameters[0].Name);

            var addRole = rich.Actions.ElementAt(2);
            Assert.AreEqual("Add Role", addRole.Name);
            Assert.AreEqual(1, addRole.Parameters.Count());
            Assert.AreEqual("Role", addRole.Parameters[0].Name);
        }

        [TestMethod, TestCategory("UserAndRoleTest")]
        public void SpecifyUsersOrganisation()
        {
            var test1 = Users.GetAction("Find User By User Name").InvokeReturnObject("Test1");
            var orgs = test1.GetPropertyByName("Organisations").ContentAsCollection.AssertCountIs(0);

            var findOrg = MockOrgs.GetAction("Find By Key");
            var alpha = findOrg.InvokeReturnObject(1).AssertTitleEquals("Alpha");
            test1.GetAction("Add Organisation").InvokeReturnObject(alpha);
            test1.GetPropertyByName("Organisations").ContentAsCollection.AssertCountIs(1).ElementAt(0).AssertTitleEquals("Alpha");

            var beta = findOrg.InvokeReturnObject(2).AssertTitleEquals("Beta");

            test1.GetAction("Add Organisation").InvokeReturnObject(beta);
            test1.GetPropertyByName("Organisations").ContentAsCollection.AssertCountIs(2).ElementAt(1).AssertTitleEquals("Beta");
  
        }

        [TestMethod, TestCategory("UserAndRoleTest")]
        public void RetrieveAllUsersForOrganisation()
        {
            var findUser = Users.GetAction("Find User By User Name");
            var test2 =findUser.InvokeReturnObject("Test2");
            var test3 = findUser.InvokeReturnObject("Test3");

            var findOrg = MockOrgs.GetAction("Find By Key");
            var epsilon = findOrg.InvokeReturnObject(5).AssertTitleEquals("Epsilon");

            test2.GetAction("Add Organisation").InvokeReturnObject(epsilon);
            test3.GetAction("Add Organisation").InvokeReturnObject(epsilon);

			var betaUsers = epsilon.GetAction("List Users").InvokeReturnCollection();
			betaUsers.AssertCountIs(2);
			Assert.AreEqual(test2, betaUsers.ElementAt(0));
			Assert.AreEqual(test3, betaUsers.ElementAt(1));
        }

        [TestMethod, TestCategory("UserAndRoleTest")]
        public void AddUserFromWithinAnOrganisationObject()
        {
            var findOrg = MockOrgs.GetAction("Find By Key");
            var gamma = findOrg.InvokeReturnObject(3).AssertTitleEquals("Gamma");

			var addUserToGamma = gamma.GetAction("Add User");
           Assert.AreEqual(1, addUserToGamma.Parameters.Count());
           addUserToGamma.Parameters[0].AssertIsNamed("User Name").AssertIsMandatory();

            //Try with a non-existent UserName
           addUserToGamma.AssertIsInvalidWithParms("Test986597543");
           //addUserToGamma.AssertLastMessageIs("Either the UserName is unknown; or the user is already associated with another organisation");

            //Now execute correctly
			var test4 =  addUserToGamma.InvokeReturnObject("Test4");
			test4.GetPropertyByName("Organisations").ContentAsCollection.AssertCountIs(1).ElementAt(0).AssertTitleEquals("Gamma");

            //Test that same user can't be added again
			addUserToGamma.AssertIsInvalidWithParms("Test4");
			addUserToGamma.AssertLastMessageContains("User is already associated with the organisation");
        }

        [TestMethod, TestCategory("UserAndRoleTest")]
        public void CanAddUserToMultipleOrgs()
        {
            var findOrg = MockOrgs.GetAction("Find By Key");
            var gamma = findOrg.InvokeReturnObject(3).AssertTitleEquals("Gamma");
            var addUserToGamma = gamma.GetAction("Add User");
            var delta = findOrg.InvokeReturnObject(4).AssertTitleEquals("Delta");
            var addUserToDelta = delta.GetAction("Add User");
            addUserToDelta.InvokeReturnObject("Test5");
			addUserToGamma.InvokeReturnObject("Test5");     
        }

    }
}
