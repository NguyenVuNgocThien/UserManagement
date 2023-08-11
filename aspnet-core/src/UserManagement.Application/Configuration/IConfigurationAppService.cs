using System.Threading.Tasks;
using UserManagement.Configuration.Dto;

namespace UserManagement.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
