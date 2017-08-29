using System.Security.Principal;
using NakedObjects.Security;

namespace Helpers.nof9
{
    /// <summary>
    /// For use in testing authorization within XATs; authorizes ALL requests
    /// </summary>
    public class TestDefaultAuthorizer : ITypeAuthorizer<object>
    {

        public void Init() { }


        public bool IsEditable(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName)
        {
            return true;
        }

        public void Shutdown() { }
    }
}
