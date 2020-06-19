using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EIRA.MultiTenancy.Dto;

namespace EIRA.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
