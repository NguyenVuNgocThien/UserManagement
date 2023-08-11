using System.Threading.Tasks;
using Abp.Application.Services;
using UserManagement.Sessions.Dto;

namespace UserManagement.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
