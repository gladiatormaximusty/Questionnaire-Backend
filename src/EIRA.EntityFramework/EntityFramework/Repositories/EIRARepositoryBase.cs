using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace EIRA.EntityFramework.Repositories
{
    public abstract class EIRARepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<EIRADbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected EIRARepositoryBase(IDbContextProvider<EIRADbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class EIRARepositoryBase<TEntity> : EIRARepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected EIRARepositoryBase(IDbContextProvider<EIRADbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
