using Abp.Application.Services;
using UserManagement.MultiTenancy.Dto;

namespace UserManagement.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

