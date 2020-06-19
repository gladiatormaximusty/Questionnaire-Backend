using Abp.Domain.Entities;
using Abp.EntityFramework;
using EIRA.IRepositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace EIRA.EntityFramework.Repositories
{
    /// <summary>
    /// 此文件不允許修改，遵循https://entityframework-plus.net/ Community Features
    /// https://entityframework.net/zh-TW/knowledge-base/42345300/
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EIRARepository<TEntity> : EIRARepositoryBase<TEntity>, IEIRARepository<TEntity> where TEntity : class, IEntity<int>
    {
        public EIRARepository(IDbContextProvider<EIRADbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public void BulkInsert(IList<TEntity> entities)
        {
            if (!entities.Any())
            {
                return;
            }
            if (GetDbContext().Database.Connection.State == ConnectionState.Closed)
            {
                GetDbContext().Database.Connection.Open();
            }

            var connection = ((SqlConnection)GetDbContext().Database.Connection);
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.BatchSize = entities.Count;
                bulkCopy.DestinationTableName = GetTableName(typeof(TEntity), GetDbContext());

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(TEntity))

                    .Cast<PropertyDescriptor>()
                    .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                    .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in entities)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
                bulkCopy.WriteToServer(table);
            }
        }

        public int BatchDelete(Expression<Func<TEntity, bool>> predicate)
        {
            return GetDbContext().Set<TEntity>().Where(predicate).Delete();
        }

        public int DeleteByKey(TEntity entityOrKeyValue)
        {
            return GetDbContext().Set<TEntity>().DeleteByKey(entityOrKeyValue);
        }

        public int DeleteByKey(params object[] keyValues)
        {
            return GetDbContext().Set<TEntity>().DeleteByKey(keyValues);
        }

        public int DeleteRangeByKey(IEnumerable<TEntity> entities)
        {
            if(!entities.Any())
            {
                return 0;
            }
            return GetDbContext().Set<TEntity>().DeleteRangeByKey(entities);
        }

        public int BatchDelete(Expression<Func<TEntity, bool>> predicate, int batchSize)
        {
            return GetDbContext().Set<TEntity>().Where(predicate).Delete(x => x.BatchSize = batchSize);
        }

        public int BatchUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return GetDbContext().Set<TEntity>().Where(predicate).Update(updateFactory);
        }

        private string GetTableName(Type type, DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));
            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
            .GetItems<EntityType>(DataSpace.OSpace)
            .Single(e => objectItemCollection.GetClrType(e) == type);
            // Get the entity set that uses this entity type
            var entitySet = metadata
            .GetItems<EntityContainer>(DataSpace.CSpace)
            .Single()
            .EntitySets
            .Single(s => s.ElementType.Name == entityType.Name);
            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
            .Single()
            .EntitySetMappings
            .Single(s => s.EntitySet == entitySet);
            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
            .EntityTypeMappings.Single()
            .Fragments.Single()
            .StoreEntitySet;
            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }
    }
}
