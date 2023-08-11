using Abp.Authorization;
using UserManagement.Authorization.Roles;
using UserManagement.Authorization.Users;

namespace UserManagement.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
