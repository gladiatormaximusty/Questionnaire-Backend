using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EIRA.ResultDto;
using EIRA.Roles.Dto;
using EIRA.Users.Dto;
using System.Threading.Tasks;

namespace EIRA.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        /// <summary>
        /// 取得用戶資料
        /// </summary>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<UserInfo>> GetUserInfoAsync(NullableIdDto input);

        /// <summary>
        /// 更新用戶資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<UserInfo>> InsertOrUpdateAsync(UserInfo input);

        /// <summary>
        /// 獲取用戶列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<PagedResultDto<UserAllOutput>>> GetPagedAllAsync(UserAllInput input);

        /// <summary>
        /// 禁用用戶
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ResultsDto<bool>> DisableAsync(EntityDto<long> input);

        /// <summary>
        /// GetProfileInfo
        /// </summary>
        /// <returns></returns>
        Task<ResultsDto<ProfileInfoOutput>> GetProfileInfoAsync();
    }
}