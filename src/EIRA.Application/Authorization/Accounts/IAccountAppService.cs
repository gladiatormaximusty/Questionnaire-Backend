using Abp.Application.Services;
using Abp.Web.Models;
using EIRA.Authorization.Accounts.Dto;
using EIRA.ResultDto;
using System.Threading.Tasks;

namespace EIRA.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);

        /// <summary>
        /// Admin登入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<LoginOutput>> AdminLogin(LoginInput input);

        /// <summary>
        /// 普通用戶登入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<LoginOutput>> Login(LoginInput input);

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        //[DontWrapResult]
        ResultsDto<bool> Logout();

        /// <summary>
        /// 忘记密码邮箱提交
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<bool>> ForgetPassword(ForgetPasswordInput input);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<bool>> ResetPassword(ResetPPasswordInput input);
    }
}
