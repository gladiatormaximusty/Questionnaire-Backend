using Abp.Castle.Logging.Log4Net;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Web;
using Abp.Web.Configuration;
using Abp.Web.Localization;
using Castle.Facilities.Logging;
using EIRA.Web.App_Start;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

namespace EIRA.Web
{
    public class MvcApplication : AbpWebApplication<EIRAWebModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseAbpLog4Net().WithConfig(Server.MapPath("log4net.config"))
            );

            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();

            //GlobalConfiguration.Configuration.Filters.Add(new WebApiExceptionFilterAttribute());

            base.Application_Start(sender, e);

            //try
            //{
            //    //new AbpApiValidationFilter(AbpBootstrapper.IocManager, null).ExecuteActionFilterAsync(null, default(CancellationToken), null);

            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception);
            //    throw;
            //}
        }

        protected override void SetCurrentCulture()
        {
            AbpBootstrapper.IocManager.Using<AppCurrentCultureSetter>(cultureSetter => cultureSetter.SetCurrentCulture(Context));
        }
    }

    public class AppCurrentCultureSetter : CurrentCultureSetter, ITransientDependency, ICurrentCultureSetter
    {
        private readonly IAbpWebLocalizationConfiguration _webLocalizationConfiguration;
        private readonly ISettingManager _settingManager;
        private readonly IAbpSession _abpSession;

        public AppCurrentCultureSetter(IAbpWebLocalizationConfiguration webLocalizationConfiguration,
            ISettingManager settingManager,
            IAbpSession abpSession) : base(webLocalizationConfiguration, settingManager, abpSession)
        {
            _webLocalizationConfiguration = webLocalizationConfiguration;
            _settingManager = settingManager;
            _abpSession = abpSession;
        }

        public override void SetCurrentCulture(HttpContext httpContext)
        {
            if (IsCultureSpecifiedInGlobalizationConfig())
            {
                return;
            }

            // 1: Query String
            var culture = GetCultureFromQueryString(httpContext);
            if (culture != null)
            {
                SetCurrentCulture(culture);
                return;
            }

            // 3 & 4: Header / Cookie
            culture = GetCultureFromHeader(httpContext) ?? GetCultureFromCookie(httpContext);
            if (culture != null)
            {
                SetCurrentCulture(culture);
                return;
            }

            // 5 & 6: Default / Browser
            culture = GetDefaultCulture() ?? GetBrowserCulture(httpContext);
            if (culture != null)
            {
                SetCurrentCulture(culture);
                SetCultureToCookie(httpContext, culture);
            }
        }
    }
}
