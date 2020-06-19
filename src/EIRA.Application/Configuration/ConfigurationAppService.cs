using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using EIRA.Configuration.Dto;

namespace EIRA.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : EIRAAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
