using System.Threading.Tasks;
using Abp.Application.Services;
using EIRA.Configuration.Dto;

namespace EIRA.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}