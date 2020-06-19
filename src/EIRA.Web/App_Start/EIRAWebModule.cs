using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.Modules;
using Abp.Threading.BackgroundWorkers;
using Abp.Web.Mvc;
using Abp.Web.SignalR;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using EIRA.Api;
using EIRA.EntityFramework.Repositories;
using EIRA.HangFire;
using EIRA.IRepositories;
using EIRA.JobManager;
using EIRA.Table;
using Hangfire;
using Microsoft.Owin.Security;

namespace EIRA.Web
{
    [DependsOn(
        typeof(EIRADataModule),
        typeof(EIRAApplicationModule),
        typeof(EIRAWebApiModule),
        typeof(AbpWebSignalRModule),
        typeof(AbpHangfireModule), //- ENABLE TO USE HANGFIRE INSTEAD OF DEFAULT JOB MANAGER
        typeof(AbpWebMvcModule))]
    public class EIRAWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Enable database based localization
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<EIRANavigationProvider>();

            //Configure Hangfire - ENABLE TO USE HANGFIRE INSTEAD OF DEFAULT JOB MANAGER
            Configuration.BackgroundJobs.UseHangfire(configuration =>
            {
                configuration.GlobalConfiguration.UseSqlServerStorage("Default");
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.IocContainer.Register(
                Component
                    .For<IAuthenticationManager>()
                    .UsingFactoryMethod(() => HttpContext.Current.GetOwinContext().Authentication)
                    .LifestyleTransient(),
                Component
                    .For<IEIRARepository<QuestionnairesAsign>>()
                    .ImplementedBy<EIRARepository<QuestionnairesAsign>>(),
                Component
                    .For<IEIRARepository<QuestionsAsign>>()
                    .ImplementedBy<EIRARepository<QuestionsAsign>>()
            );
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            //workManager.Add(IocManager.Resolve<JobWorker>());
            RecurringJob.AddOrUpdate<IWorkerAppService>("ChangeQuestionnaires", x => x.ChangeQuestionnaires(), "0 0 0 * * ? ", TimeZoneInfo.Local);
        }
    }
}
