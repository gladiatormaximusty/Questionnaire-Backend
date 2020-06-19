using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using EIRA.Authorization.Users;
using EIRA.MultiTenancy;
using EIRA.Users;
using Microsoft.AspNet.Identity;

namespace EIRA
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class EIRAAppServiceBase : ApplicationService
    {
        //URL特殊字符
        public const string UrlRegex = "[+ /?%#&=]";

        /// <summary>
        /// 常量，表示成功
        /// </summary>
        public const int Succeed = 200;

        /// <summary>
        /// 常量，表示返回InternalServerError错误:业务错误
        /// </summary>
        public const int InternalServerError = 502;

        /// <summary>
        /// 常量，表示系统错误
        /// </summary>
        public const int SystemError = 500;

        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected EIRAAppServiceBase()
        {
            LocalizationSourceName = EIRAConsts.LocalizationSourceName;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}