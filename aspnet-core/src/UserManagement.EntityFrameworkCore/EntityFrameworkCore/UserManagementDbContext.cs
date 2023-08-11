using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using UserManagement.Authorization.Roles;
using UserManagement.Authorization.Users;
using UserManagement.MultiTenancy;

namespace UserManagement.EntityFrameworkCore
{
    public class UserManagementDbContext : AbpZeroDbContext<Tenant, Role, User, UserManagementDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options)
        {
        }
    }
}
