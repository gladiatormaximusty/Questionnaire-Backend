using System.Threading.Tasks;
using Abp.Application.Services;
using EIRA.Sessions.Dto;

namespace EIRA.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
