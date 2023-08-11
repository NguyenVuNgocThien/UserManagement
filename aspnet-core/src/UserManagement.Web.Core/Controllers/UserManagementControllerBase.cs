using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Controllers
{
    public abstract class UserManagementControllerBase: AbpController
    {
        protected UserManagementControllerBase()
        {
            LocalizationSourceName = UserManagementConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
