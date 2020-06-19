using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using EIRA.EntityFramework;

namespace EIRA.Migrator
{
    [DependsOn(typeof(EIRADataModule))]
    public class EIRAMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<EIRADbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}