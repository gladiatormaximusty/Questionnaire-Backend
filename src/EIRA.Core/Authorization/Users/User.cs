using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Authorization.Users;
using Abp.Extensions;
using EIRA.Table;
using Microsoft.AspNet.Identity;

namespace EIRA.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress, string password)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Password = new PasswordHasher().HashPassword(password),
                IsAdmin=true
            };

            user.SetNormalizedNames();

            return user;
        }


        [MaxLength(50)]
        public string Account { get; set; }

        [MaxLength(1024 * 64)]
        public string Image { get; set; }

        public int? BU_Id { get; set; }

        [ForeignKey("BU_Id")]
        public BUs BU { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        public bool IsForceChangPwd { get; set; }
        public bool IsUpdateImage { get; set; }
    }
}