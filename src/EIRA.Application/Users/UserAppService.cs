using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Net.Mail.Smtp;
using Abp.ObjectMapping;
using EIRA.Authorization;
using EIRA.Authorization.Roles;
using EIRA.Authorization.Users;
using EIRA.Common;
using EIRA.Enums;
using EIRA.ResultDto;
using EIRA.Roles.Dto;
using EIRA.Table;
using EIRA.TableManager;
using EIRA.Users.Dto;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;

namespace EIRA.Users
{
    [AbpAuthorize]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>, IUserAppService
    {
        private string Domain = ConfigurationSettings.AppSettings["Domain"];

        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IObjectMapper _objectManager;
        private readonly IRepository<BUs> _busRepository;
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;
        private readonly ISmtpEmailSenderConfiguration _smtpEmialSenderConfig;
        AdminUserManager _adminUserManager;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            IRepository<Role> roleRepository,
            IObjectMapper objectManager,
            IRepository<BUs> busRepository,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            RoleManager roleManager, AdminUserManager adminUserManager,
            ISmtpEmailSenderConfiguration smtpEmialSenderConfig)
            : base(repository)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _roleManager = roleManager;
            _objectManager = objectManager;
            _busRepository = busRepository;
            _userLoginAttemptRepository = userLoginAttemptRepository;
            _adminUserManager = adminUserManager;
            _smtpEmialSenderConfig = smtpEmialSenderConfig;
        }

        public override async Task<UserDto> GetAsync(EntityDto<long> input)
        {
            var user = await base.GetAsync(input);
            var userRoles = await _userManager.GetRolesAsync(user.Id);
            user.Roles = userRoles.Select(ur => ur).ToArray();
            return user;
        }

        public override async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.Password = new PasswordHasher().HashPassword(input.Password);
            user.IsEmailConfirmed = true;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.RoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            CheckErrors(await _userManager.CreateAsync(user));

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(user);
        }

        public override async Task<UserDto> UpdateAsync(UpdateUserDto input)
        {
            CheckUpdatePermission();

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return await GetAsync(input);
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            return user;
        }

        protected override void MapToEntity(UpdateUserDto input, User user)
        {
            ObjectMapper.Map(input, user);
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = Repository.GetAllIncluding(x => x.Roles).FirstOrDefault(x => x.Id == id);
            return await Task.FromResult(user);
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        #region Get Profile Information

        /// <summary>
        /// Get Profile Info
        /// </summary>
        /// <returns></returns>
        public async Task<ResultsDto<ProfileInfoOutput>> GetProfileInfoAsync()
        {
            ResultsDto<ProfileInfoOutput> resultDto = new ResultsDto<ProfileInfoOutput>();

            try
            {
                var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);

                var login = await _userLoginAttemptRepository.GetAll().Where(x => x.Result == AbpLoginResultType.Success && x.UserId == AbpSession.UserId.Value).FirstOrDefaultAsync();

                resultDto.Data = new ProfileInfoOutput()
                {
                    Id = user.Id,
                    EmailAddress = user.EmailAddress,
                    Image = user.Image,
                    Name = user.Name,
                    Surname = user.Surname,
                    CreationTime = user.CreationTime.ToEIRATime(),
                    LastModificationTime = user.LastModificationTime?.ToEIRATime(),
                    LastLogin = login?.CreationTime.ToEIRATime()
                };

                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        #endregion

        #region GetUserInfoAsync

        [AbpAuthorize(PermissionNames.Pages_Admin)]
        public async Task<ResultsDto<UserInfo>> GetUserInfoAsync(NullableIdDto input)
        {
            ResultsDto<UserInfo> resultDto = new ResultsDto<UserInfo>();

            try
            {
                if (input.Id == 0)
                {
                    UserInfo userInfo = new UserInfo();

                    //add new User頁面返回UpdateTime,UpdateUser給前端顯示
                    userInfo.UpdateTime = DateTime.Now.ToEIRATime();
                    userInfo.UpdateUser = _adminUserManager.GetUserNameById(AbpSession.UserId);

                    //預設圖片
                    userInfo.Image = Domain + "Images/eira-img-face.svg";

                    resultDto.Data = userInfo;
                }
                else
                {
                    var user = await _userManager.Users.Where(x => x.Id == input.Id).Include(o => o.BU).FirstOrDefaultAsync();
                    resultDto.Data = _objectManager.Map<UserInfo>(user);

                    resultDto.Data.Status = user.IsActive ? UserStatus.Active.ToString() : UserStatus.InActive.ToString();

                    if (user != null)
                    {
                        //最後更新信息：如果沒有修改人，則顯示創建人信息
                        if (user.LastModificationTime.HasValue)
                        {
                            resultDto.Data.UpdateTime = user.LastModificationTime.Value.ToEIRATime();
                            resultDto.Data.UpdateUser = _adminUserManager.GetUserNameById(user.LastModifierUserId);
                        }
                        else
                        {
                            resultDto.Data.UpdateTime = user.CreationTime.ToEIRATime();
                            resultDto.Data.UpdateUser = _adminUserManager.GetUserNameById(user.CreatorUserId);
                        }
                    }

                    if (resultDto.Data != null && string.IsNullOrWhiteSpace(resultDto.Data.Image))
                    {
                        resultDto.Data.Image = Domain + "Image/eira-img-face.svg";
                    }
                    resultDto.Data.IsCurrentLoginUser = user.Id == AbpSession.UserId;
                }

                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        #endregion

        #region 新增/修改用戶

        /// <summary>
        /// 新增/修改用戶
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_Admin)]
        public async Task<ResultsDto<UserInfo>> InsertOrUpdateAsync(UserInfo input)
        {
            CheckUpdatePermission();

            ResultsDto<UserInfo> resultDto = new ResultsDto<UserInfo>();

            try
            {

                List<string> ErrorMessage = new List<string>();

                #region 必要項驗證

                if (input.BU_Id == null || input.BU_Id == 0)
                {
                    ErrorMessage.Add("Please select a BU!");
                }

                #endregion

                #region 唯一性驗證

                //User Id不能重複
                if (_userManager.Users.Where(x => x.Id != input.Id && x.Account == input.Account.Trim()).Any())
                {
                    ErrorMessage.Add(string.Format("Already have the same User Id:{0}, Please re-enter! ", input.Account.Trim()));
                }

                //EmailAddress不能重複
                if (_userManager.Users.Where(x => x.Id != input.Id && x.EmailAddress == input.EmailAddress.Trim()).Any())
                {
                    ErrorMessage.Add(string.Format("Already have the same Email:{0}, Please re-enter! ", input.EmailAddress.Trim()));
                }

                #endregion

                if (ErrorMessage.Any())
                {
                    resultDto.Status.Code = 502;
                    resultDto.Status.Message = string.Join("\r\n", ErrorMessage);

                    return resultDto;
                }

                var user = _userManager.Users.Where(x => x.Id == input.Id).FirstOrDefault();
                var newPwd = StrHelper.RandomStr(8);
                var isSendEmail = false;

                if (user == null)
                {
                    //新增用戶
                    user = new User();
                    user.Password = new PasswordHasher().HashPassword(newPwd);
                    user.IsForceChangPwd = true;
                    isSendEmail = true;
                }
                else
                {
                    if(input.EmailAddress != user.EmailAddress)
                    {
                        user.Password = new PasswordHasher().HashPassword(newPwd);
                        isSendEmail = true;
                    }
                }

                if (input.BU_Id.HasValue && input.BU_Id != 0)
                {
                    user.BU = await _busRepository.GetAsync(input.BU_Id.Value);
                }

                //角色
                user.Roles = new List<UserRole>();
                if (input.IsAdmin)
                {
                    foreach (var defaultRole in await _roleManager.Roles.Where(r => r.DisplayName == "Admin").ToListAsync())
                    {
                        user.Roles.Add(new UserRole(null, user.Id, defaultRole.Id));
                    }
                }
                else
                {
                    CheckErrors(await _userManager.SetRoles(user, new string[] { }));
                }

                #region 頭像設置

                //如果用戶有上傳頭像，則使用當前修改的頭像
                if (!string.IsNullOrWhiteSpace(input.Image) && input.Image != user.Image)
                {
                    user.Image = input.Image;
                    user.IsUpdateImage = true;
                }
                else
                {
                    //如果清空頭像或修改姓名則重新添加默認頭像（由姓名首字母生成的頭像）
                    if ((string.IsNullOrWhiteSpace(input.Image) || input.Name != user.Name || input.Surname != user.Surname) && !user.IsUpdateImage)
                    {
                        //生成文字頭像
                        string shortName = input.Name.Substring(0, 1) + input.Surname.Substring(0, 1);
                        //生成的Domain(local端設定加上/)
                        user.Image = Domain +"/" + FileHelper.GenerateImage(shortName);
                    }
                }

                #endregion

                if (isSendEmail)
                {
                    #region SendEmail（新建用戶或变更會自動產生密碼，並發到user email帳戶，用於登入系統）

                    var title = await SettingManager.GetSettingValueAsync("MailTitle");
                    var content = await SettingManager.GetSettingValueAsync("MailCreatUserContent");

                    //发送邮件
                    SmtpEmailSender emailSender = new SmtpEmailSender(_smtpEmialSenderConfig);
                    string message = content + "<lable style=\"color: red\">{0}</lable>";
                    var sendMsg = string.Format(message, newPwd);
                    emailSender.Send(_smtpEmialSenderConfig.UserName, input.EmailAddress, title, sendMsg);

                    #endregion
                }

                user.UserName = input.EmailAddress;
                user.Name = input.Name;
                user.Surname = input.Surname;
                user.EmailAddress = input.EmailAddress;
                user.IsActive = input.Status == UserStatus.Active.ToString() ? true : false;
                user.IsAdmin = input.IsAdmin;
                user.Account = input.Account;

                if (input.Id > 0)
                {
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    await _userManager.CreateAsync(user);
                }

                resultDto.Data = _objectManager.Map<UserInfo>(user);

                resultDto.Data.Status = user.IsActive ? UserStatus.Active.ToString() : UserStatus.InActive.ToString();

                if (user.LastModificationTime.HasValue)
                {
                    resultDto.Data.UpdateTime = user.LastModificationTime.Value.ToEIRATime();
                    resultDto.Data.UpdateUser = _adminUserManager.GetUserNameById(user.LastModifierUserId);
                }

                resultDto.Data.IsCurrentLoginUser= user.Id== AbpSession.UserId;

                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = e.Message;
            }

            return resultDto;
        }

        #endregion

        #region 獲取所有用戶

        /// <summary>
        /// 獲取所有用戶
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_Admin)]
        public async Task<ResultsDto<PagedResultDto<UserAllOutput>>> GetPagedAllAsync(UserAllInput input)
        {
            ResultsDto<PagedResultDto<UserAllOutput>> resultDto = new ResultsDto<PagedResultDto<UserAllOutput>>();

            try
            {
                var query = _userManager.Users.Include(o => o.BU);

                if (!string.IsNullOrWhiteSpace(input.Keyword))
                {
                    query = query.Where(x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword) || x.Account.Contains(input.Keyword));
                }

                //排序
                if (string.IsNullOrWhiteSpace(input.Sorting))
                {
                    input.Sorting = "Id desc";
                }

                var count = await query.CountAsync();

                var dtos = await query.Select(o => new UserAllOutput
                {
                    Id = o.Id,
                    Name = o.Name + " " + o.Surname,
                    BU = o.BU.BUName,
                    Code = o.Account,
                    Status = o.IsActive ? UserStatus.Active.ToString() : UserStatus.InActive.ToString()
                }).OrderBy(input.Sorting).PageBy(input).ToListAsync();

                resultDto.Data = new PagedResultDto<UserAllOutput>(count, dtos);

                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway);
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        #endregion

        #region DisableAsync

        [AbpAuthorize(PermissionNames.Pages_Admin)]
        public async Task<ResultsDto<bool>> DisableAsync(EntityDto<long> input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            try
            {
                var user = await _userManager.GetUserByIdAsync(input.Id);
                user.IsActive = false;
                await _userManager.UpdateAsync(user);

                resultDto.Data = true;
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Data = false;
                resultDto.Status.Code = Convert.ToInt32(HttpStatusCode.BadGateway); ;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        #endregion
    }
}