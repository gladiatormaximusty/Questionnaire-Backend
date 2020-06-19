using System.Collections.Generic;
using Abp.Configuration;

namespace EIRA.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition(
                        "MailTitle",
                        "Retrieve password",
                        scopes: SettingScopes.Application
                        ),
                new SettingDefinition(
                        "MailResetContent",
                        "You are operating to retrieve the password. If you are not yourself, please ignore it. To continue, please click the link below ",
                        scopes: SettingScopes.Application
                        ),
                new SettingDefinition(
                        "MailCreatUserContent",
                        "Your account has been created with an initial password of ",
                        scopes: SettingScopes.Application
                        ),
                new SettingDefinition(
                        "MailReSetUrl",
                        "",
                        scopes: SettingScopes.Application
                        )
            };
        }
    }
}