using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using UserManagement.Authorization;

namespace UserManagement
{
    [DependsOn(
        typeof(UserManagementCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class UserManagementApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<UserManagementAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(UserManagementApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
