using System.Collections.Generic;
using System.Linq;
namespace Cluster.Users.Api
{
    public interface IUserService
    {
        IUser CurrentUser();

        IUser FindUserById(int id);

        /// <summary>
        /// Returns the user object with the given name, only if an exact match is found.
        /// </summary>
        IUser FindUserByUserName(string userName);

        /// <summary>
        /// Finds users where there is a partial match on either the real or user names.
        /// If a role is specified, returns only users with that role.
        /// </summary>
        IQueryable<IUser> FindUsersByRealOrUserName(string match);

        /// <summary>
        /// Returns true if the userName exists and that user is not already associated with
        /// any other organisation.  If its returns false then an errorMessage will be provided
        /// in a form that can be rendered to the user.
        /// </summary>
        bool CanAddUser(string userName, IUserOrg toOrg, out string errorMessage);

        /// <summary>
        /// Assumes that CanAddUser has already been called.  So will fail with exception if
        /// the pre-conditions are not met.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="toOrg"></param>
        IUser AddUser(string userName, IUserOrg toOrg);

        IQueryable<IUser> ListUsersForOrganisation(IUserOrg organisation);

        bool CurrentUserCanActFor(IUserOrg organisation);

        ICollection<IUserOrg> OrganisationsOfCurrentUser();
    }
}
