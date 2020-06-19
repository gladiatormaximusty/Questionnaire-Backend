using Abp.Authorization;
using EIRA.Authorization.Roles;
using EIRA.Authorization.Users;

namespace EIRA.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
