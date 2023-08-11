using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using UserManagement.EntityFrameworkCore;
using UserManagement.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace UserManagement.Web.Tests
{
    [DependsOn(
        typeof(UserManagementWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class UserManagementWebTestModule : AbpModule
    {
        public UserManagementWebTestModule(UserManagementEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(UserManagementWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(UserManagementWebMvcModule).Assembly);
        }
    }
}