using Abp.DynamicEntityParameters;
using Abp.Zero.EntityFramework;
using EIRA.Authorization.Roles;
using EIRA.Authorization.Users;
using EIRA.MultiTenancy;
using EIRA.Table;
using System.Data.Common;
using System.Data.Entity;

namespace EIRA.EntityFramework
{
    public class EIRADbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...

        public IDbSet<BUs> BUs { get; set; }
        public IDbSet<Entities> Entities { get; set; }
        public IDbSet<QuestionTypes> QuestionTypes { get; set; }
        public IDbSet<Questions> Questions { get; set; }
        public IDbSet<QuestionsAnswer> QuestionsAnswer { get; set; }
        public IDbSet<Questionnaires> Questionnaires { get; set; }
        public IDbSet<QuestionnairesAsign> QuestionnairesAsign { get; set; }
        public IDbSet<SupportingDocument> SupportingDocument { get; set; }
        public IDbSet<QuestionnairesFinished> QuestionnairesFinished { get; set; }
        public IDbSet<QuestionsAsign> QuestionsAsign { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public EIRADbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in EIRADataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of EIRADbContext since ABP automatically handles it.
         */
        public EIRADbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public EIRADbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public EIRADbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DynamicParameter>().Property(p => p.ParameterName).HasMaxLength(250);
            modelBuilder.Entity<EntityDynamicParameter>().Property(p => p.EntityFullName).HasMaxLength(250);
        }
    }
}
