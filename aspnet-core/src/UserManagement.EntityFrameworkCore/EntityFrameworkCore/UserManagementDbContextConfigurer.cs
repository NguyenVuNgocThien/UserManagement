using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.EntityFrameworkCore
{
    public static class UserManagementDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<UserManagementDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<UserManagementDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
