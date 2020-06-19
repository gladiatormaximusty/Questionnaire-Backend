using EIRA.EntityFramework;
using EntityFramework.DynamicFilters;

namespace EIRA.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly EIRADbContext _context;

        public InitialHostDbBuilder(EIRADbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
