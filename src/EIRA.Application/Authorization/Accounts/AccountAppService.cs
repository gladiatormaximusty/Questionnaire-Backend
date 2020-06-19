using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.UI;
using Abp.Zero.Configuration;
using EIRA.Authorization.Accounts.Dto;
using EIRA.Authorization.Users;
using EIRA.MultiTenancy;
using EIRA.ResultDto;
using Microsoft.Owin.Security;
using System;
using System.Net;
using System.Linq;
using System.Data.Entity;
using EIRA.Common;
using Abp.Net.Mail.Smtp;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace EIRA.Authorization.Accounts
{
    public class AccountAppService : EIRAAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserLoginManager _userLoginManager;
        private readonly LogInManager _logInManager;
        private readonly ISmtpEmailSenderConfiguration _smtpEmialSenderConfig;
        public const string PasswordRegex = "^(?=.*[0-9])(?=.*[a-zA-Z])(.{8,20})$";

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            UserLoginManager userLoginManager,
            LogInManager logInManager,
            ISmtpEmailSenderConfiguration smtpEmialSenderConfig)
        {
            _userRegistrationManager = userRegistrationManager;
            _logInManager = logInManager;
            _userLoginManager = userLoginManager;
            _smtpEmialSenderConfig = smtpEmialSenderConfig;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                false
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        #region Login/Logput

        public async Task<ResultsDto<LoginOutput>> AdminLogin(LoginInput input)
        {
            ResultsDto<LoginOutput> resultDto = new ResultsDto<LoginOutput>();

            try
            {
                var loginResult = await GetLoginResultAsync(input.Account, input.Password);

                if (!loginResult.User.IsAdmin)
                {
                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                    resultDto.Status.Message = "You are not an administrator and cannot login to this system!";

                    return resultDto;
                }
                else
                {
                    var ticket = new AuthenticationTicket(loginResult.Identity, new AuthenticationProperties());

                    var currentUtc = DateTime.UtcNow;
                    ticket.Properties.IssuedUtc = currentUtc;
                    ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(1));

                    LoginOutput loginOutput = new LoginOutput();

                    loginOutput.Token = UserLoginManager.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
                    loginOutput.IsForceChangPwd = loginResult.User.IsForceChangPwd;
                    loginOutput.Image = loginResult.User.Image;
                    loginOutput.CurrentUser = loginResult.User.Name + " " + loginResult.User.Surname;

                    resultDto.Data = loginOutput;

                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = e.Message;
            }

            return resultDto;
        }

        public async Task<ResultsDto<LoginOutput>> Login(LoginInput input)
        {
            ResultsDto<LoginOutput> resultDto = new ResultsDto<LoginOutput>();

            try
            {
                var loginResult = await GetLoginResultAsync(input.Account, input.Password);

                var ticket = new AuthenticationTicket(loginResult.Identity, new AuthenticationProperties());

                var currentUtc = DateTime.UtcNow;
                ticket.Properties.IssuedUtc = currentUtc;
                ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(1));

                LoginOutput loginOutput = new LoginOutput();

                loginOutput.Token = UserLoginManager.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
                loginOutput.IsForceChangPwd = loginResult.User.IsForceChangPwd;
                loginOutput.Image = loginResult.User.Image;
                loginOutput.CurrentUser = loginResult.User.Name + " " + loginResult.User.Surname;

                resultDto.Data = loginOutput;

                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = e.Message;
            }

            return resultDto;
        }

        /// <summary>
        /// 註銷
        /// </summary>
        /// <returns></returns>
        [AbpAuthorize]
        public ResultsDto<bool> Logout()
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            try
            {
                _userLoginManager.MobileUserSingOut(); ;

                resultDto.Data = true;
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
                resultDto.Status.Message = "Logout successful.";
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Data = false;
                resultDto.Status.Code = 500;
                resultDto.Status.Message = "System exception. Please try again later.";
            }

            return resultDto;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, password);
            }
        }

        private Exception CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new ApplicationException("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException(L("LoginFailed"), L("InvalidUserNameOrPassword"));
                case AbpLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException(L("LoginFailed"), L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                case AbpLoginResultType.TenantIsNotActive:
                    return new UserFriendlyException(L("LoginFailed"), L("TenantIsNotActive", tenancyName));
                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException(L("LoginFailed"), L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException(L("LoginFailed"), "Your email address is not confirmed. You can not login"); //TODO: localize message
                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + result);
                    return new UserFriendlyException(L("LoginFailed"));
            }
        }

        #endregion

        #region Forget password

        /// <summary>
        /// 忘记密码邮箱提交
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task<ResultsDto<bool>> ForgetPassword(ForgetPasswordInput input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();
            try
            {
                if (string.IsNullOrWhiteSpace(input.EmailAddr))
                {
                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                    resultDto.Status.Message = "Operation failed";
                    return resultDto;
                }

                var user = await UserManager.Users.Where(x => x.EmailAddress == input.EmailAddr).FirstOrDefaultAsync();
                if (user == null)
                {
                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                    resultDto.Status.Message = "Please enter the correct email in the field.";
                    return resultDto;
                }

                var title = await SettingManager.GetSettingValueAsync("MailTitle");
                var content = await SettingManager.GetSettingValueAsync("MailResetContent");
                var url = await SettingManager.GetSettingValueAsync("MailReSetUrl");
                var newPwd = StrHelper.RandomStr(8);

                //发送邮件
                SmtpEmailSender emailSender = new SmtpEmailSender(_smtpEmialSenderConfig);
                string message = content + "<label style=\"color: red\">{0}</label>";
                var sendMsg = string.Format(message, newPwd);
                emailSender.Send(_smtpEmialSenderConfig.UserName, input.EmailAddr, title, sendMsg);

                user.Password = new PasswordHasher().HashPassword(newPwd);
                user.IsForceChangPwd = true;

                await UserManager.UpdateAsync(user);

                resultDto.Data = true;
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Data = false;
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = e.Message;
            }

            return resultDto;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<ResultsDto<bool>> ResetPassword(ResetPPasswordInput input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            try
            {
                if (string.IsNullOrWhiteSpace(input.NewPassword))
                {
                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                    resultDto.Status.Message = "Operation failed.";
                    return resultDto;
                }

                if (input.NewPassword != input.ConfirmPassword)
                {
                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                    resultDto.Status.Message = "Please enter the same password.";
                    return resultDto;
                }

                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                    resultDto.Status.Message = "Operation error or link is invalid.";
                    return resultDto;
                }

                if (!new Regex(PasswordRegex).IsMatch(input.NewPassword))
                {
                    resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                    resultDto.Status.Message = "Password must be 8-20 characters long, must contain both letters and numbers.";
                    return resultDto;
                }

                user.Password = new PasswordHasher().HashPassword(input.NewPassword);
                user.IsForceChangPwd = false;

                await UserManager.UpdateAsync(user);

                resultDto.Data = true;
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Data = false;
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = e.Message;
            }

            return resultDto;
        }

        #endregion
    }
}