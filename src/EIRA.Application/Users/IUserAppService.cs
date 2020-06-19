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
        /// ȡ���Ñ��Y��
        /// </summary>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<UserInfo>> GetUserInfoAsync(NullableIdDto input);

        /// <summary>
        /// �����Ñ��Y��
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<UserInfo>> InsertOrUpdateAsync(UserInfo input);

        /// <summary>
        /// �@ȡ�Ñ��б�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DontWrapResult]
        Task<ResultsDto<PagedResultDto<UserAllOutput>>> GetPagedAllAsync(UserAllInput input);

        /// <summary>
        /// �����Ñ�
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