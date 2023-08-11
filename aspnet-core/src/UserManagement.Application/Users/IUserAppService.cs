using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Http;
using UserManagement.Roles.Dto;
using UserManagement.Users.Dto;

namespace UserManagement.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task DeActivate(EntityDto<long> user);
        Task Activate(EntityDto<long> user);
        Task<ListResultDto<RoleDto>> GetRoles();
        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);
        Task<string> GetFileImport(IFormFile file);
        Task<string> ImportFileAsync(string pathFile);
        Task<IDictionary<string,object>> UpdateMultiUser(List<ImportUserReqDto> input);
        Task<IDictionary<string, object>> CreateOrEditMultiUser(ImportUserReqDto input);
    }
}
