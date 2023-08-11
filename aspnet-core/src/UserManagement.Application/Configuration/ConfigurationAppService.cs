using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using UserManagement.Configuration.Dto;

namespace UserManagement.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : UserManagementAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
