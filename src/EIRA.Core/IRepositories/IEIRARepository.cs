using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EIRA.IRepositories
{
    public interface IEIRARepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        void BulkInsert(IList<TEntity> entities);

        int BatchDelete(Expression<Func<TEntity, bool>> predicate);

        int DeleteByKey(TEntity entityOrKeyValue);

        int DeleteByKey(params object[] keyValues);

        int DeleteRangeByKey(IEnumerable<TEntity> entities);

        int BatchDelete(Expression<Func<TEntity, bool>> predicate, int batchSize);

        int BatchUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateFactory);
    }
}
