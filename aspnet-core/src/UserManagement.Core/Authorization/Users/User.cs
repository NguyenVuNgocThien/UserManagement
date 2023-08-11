﻿using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace UserManagement.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateofBirth { get; set; } 
        public string CitizenIdentification { get; set; } = null;
        public string Address { get; set; } = null;
        public DateTime UpdateDate { get; set; }

    }
}
