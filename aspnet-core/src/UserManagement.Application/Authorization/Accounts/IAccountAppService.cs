using System.Threading.Tasks;
using Abp.Application.Services;
using UserManagement.Authorization.Accounts.Dto;

namespace UserManagement.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
