using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using UserManagement.Authorization;
using UserManagement.Authorization.Accounts;
using UserManagement.Authorization.Roles;
using UserManagement.Authorization.Users;
using UserManagement.Roles.Dto;
using UserManagement.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using UserManagement.EntityFrameworkCore;
using Aspose.Cells;
using UserManagement.Helpper;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using UserManagement.Asposes;
using UserManagement.Asposes.Dto;
using System.Globalization;

namespace UserManagement.Users
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService :  AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly IWebHostEnvironment _hosttingEnviroment;
        private readonly AsposesAppService _aspose;
        //private readonly UserManagementDbContext _dbContext;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            IWebHostEnvironment hosttingEnviroment,
            AsposesAppService aspose,
            //UserManagementDbContext dbContext,
            LogInManager logInManager)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _hosttingEnviroment = hosttingEnviroment;
            _aspose = aspose;
            //_dbContext = dbContext;
        }

        public override async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }

        public override async Task<UserDto> UpdateAsync(UserDto input)
        {
            CheckUpdatePermission();

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
            }

            return await GetAsync(input);
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task Activate(EntityDto<long> user)
        {
            await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = true;
            });
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task DeActivate(EntityDto<long> user)
        {
            await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = false;
            });
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roleIds = user.Roles.Select(x => x.RoleId).ToArray();

            var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.NormalizedName);

            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();

            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<bool> ChangePassword(ChangePasswordDto input)
        {
            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            var user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }
            
            if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
            {
                CheckErrors(await _userManager.ChangePasswordAsync(user, input.NewPassword));
            }
            else
            {
                CheckErrors(IdentityResult.Failed(new IdentityError
                {
                    Description = "Incorrect password."
                }));
            }

            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attempting to reset password.");
            }
            
            var currentUser = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
            }
            
            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                return false;
            }
            
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            {
                throw new UserFriendlyException("Only administrators may reset passwords.");
            }

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return true;
        }
        [HttpPost]
        public async Task<string> GetFileImport(IFormFile file)
        {
            string filePath = null;
            try
            {
                var maker = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
                string result = "";
                string path = "";
                string pathFile = Path.Combine(_hosttingEnviroment.WebRootPath, "IMPORT_USER\\" + maker.Name + "\\");
                if (!Directory.Exists(pathFile))
                {
                    Directory.CreateDirectory(pathFile);
                }
                if (file.Length > 0)
                {
                    if (Directory.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    filePath = Path.Combine(pathFile, file.FileName);
                    path = filePath;
                    using (Stream filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(filestream);
                        result = await ImportFileAsync(path);
                    }
                    File.Delete(filePath);
                }
                return result;
            }
            catch (InvalidCastException e)
            {
                Logger.Error(e.Message, e);
                if (filePath != null)
                {
                    if (Directory.Exists(filePath)) File.Delete(filePath);
                }
                return "Import Failed";
            }
        }

        public async Task<string> ImportFileAsync(string pathFile)
        {
            Workbook designer = new Workbook(pathFile);
            string result = "";
            WorksheetCollection collection = designer.Worksheets;
            var maker = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            try
            {
                var listUser = new List<ImportUserReqDto>();
                for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
                {
                    Worksheet worksheet = collection[worksheetIndex];
                    int rows = worksheet.Cells.MaxDataRow;
                    int col = worksheet.Cells.MaxDataColumn;
                    for (int i = 0; i <= rows; i++)
                    {
                        for (int j = 0; j <= col; j++)
                        {
                            if (worksheet.Cells[i, j].Value == null || string.IsNullOrEmpty(worksheet.Cells[i, j].Value.ToString()))
                            {
                                result = "Some information is empty, please fill this information at row " + i + " col " + j;
                            }
                        }
                        var user = new ImportUserReqDto();
                        user.LastName = worksheet.Cells[i, 0].Value.ToString();
                        user.FirstName = worksheet.Cells[i, 1].Value.ToString();
                        user.DateofBirth = (worksheet.Cells[i, 2].Value.ToString());
                        user.Address = worksheet.Cells[i, 3].Value.ToString();
                        user.PhoneNumber = worksheet.Cells[i, 4].Value.ToString();
                        user.CitizenIdentification = worksheet.Cells[i, 5].Value.ToString();
                        user.Email = worksheet.Cells[i, 6].Value.ToString();
                        listUser.Add(user);
                    }
                    await UpdateMultiUser(listUser);
                    result = "Success";
                }
                return result;
            }
            catch (InvalidCastException e)
            {
                Logger.Error(e.Message, e);
                return e.ToString();
            }
        }

        public async Task<IDictionary<string, object>> UpdateMultiUser(List<ImportUserReqDto> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                await CreateOrEditMultiUser(input[i]);
            }
            return new Dictionary<string, object>() { { "1", new { result = "Success" } } };
        }

        public async Task<IDictionary<string, object>> CreateOrEditMultiUser(ImportUserReqDto input)
        {
            try
            {
                var maker = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
                string[] splitFirstName = input.FirstName.Split(' ');
                string fullFirstName = "";
                foreach (string slice in splitFirstName)
                    if (slice.Length > 0)
                    {
                        fullFirstName += slice.ToString().ToLower();
                    }
                string[] splitlastname = input.LastName.Split(' ');
                string fullLastName = "";
                foreach (string slice in splitlastname)
                    if (slice.Length > 0)
                    {
                        fullLastName += slice[0].ToString().ToLower();
                    }
                var userName = fullFirstName + fullLastName;
                userName = Utilities.ChangeFormatName(userName);
                var duplicatename = await _userManager.Users.FirstOrDefaultAsync(x=>x.UserName==userName);

                int count = 0;
                string newname = userName;
                while (duplicatename != null)
                {
                    count++;
                    newname = (userName + count.ToString());
                    duplicatename = await _userManager.Users.FirstOrDefaultAsync(p => p.UserName == newname);
                }
                string format = "dd/MM/yyyy";

                DateTime dateOfBirth = DateTime.ParseExact(input.DateofBirth, format, CultureInfo.InvariantCulture);




                var newUser = new User
                {
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    DateofBirth = dateOfBirth,
                    UserName = newname,
                    Address = input.Address,
                    PhoneNumber = input.PhoneNumber,
                    CitizenIdentification = input.CitizenIdentification,
                    EmailAddress=input.Email,
                    Name=input.LastName+input.FirstName,
                    Surname="",
                    LastModificationTime=DateTime.Now
                };

                var defaultPassword = $"{newname}@{input.DateofBirth}";
                var pass = char.ToUpper(defaultPassword[0]) + defaultPassword.Substring(1);
                var result = await _userManager.CreateAsync(newUser, pass.Replace("/",""));
                return new Dictionary<string, object>() { { "1", new { result = ObjectMapper.Map<User>(newUser) } } };
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                return new Dictionary<string, object>() { { "0", new { result = "Error" } } };
            }
        }

        public async Task<FileDto> ExportFileExcel(ExcelInfoDto excelif)
        {
            try
            {
                ReportInfoDto info = new ReportInfoDto();
                info.PathName = excelif.PathName;
                if (excelif.TypeExport == "FileTypeConst.Pdf")
                {
                    info.TypeExport = FileTypeConst.Pdf;
                }
                else
                {
                    info.TypeExport = FileTypeConst.Excel;
                }
                info.PathName = excelif.PathName;
                info.StoreName = excelif.StoreName;
                var file = await _aspose.GetReport_User(info);

                FileDto fileExcel = new FileDto();
                fileExcel.FileType = file.FileType;
                fileExcel.FileToken = file.FileToken;
                fileExcel.FileName = file.FileName;
                return fileExcel;
            }
            catch(Exception e)
            {
                Logger.Error(e.Message, e);
                return null;
            }
        }

    }
}

