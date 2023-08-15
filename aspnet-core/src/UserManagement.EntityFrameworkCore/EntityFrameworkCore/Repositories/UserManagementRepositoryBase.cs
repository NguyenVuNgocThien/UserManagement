using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using System;
using System.Data;
using System.Reflection;

namespace UserManagement.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class UserManagementRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<UserManagementDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected UserManagementRepositoryBase(IDbContextProvider<UserManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // Add your common methods for all repositories
    }
    public abstract class SqlRepositoryBase<TEntity,TPrimaryKey>:EfCoreRepositoryBase<UserManagementDbContext,TEntity,TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, new()
    {
        protected SqlRepositoryBase (IDbContextProvider<UserManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        protected virtual TEntity Map(IDataReader reader)
        {
            TEntity item = new TEntity();
            DataTable dt = reader.GetSchemaTable();
            foreach(var prop in item.GetType().GetProperties())
            {
                if(IsExistColumn(dt,prop.Name)&& !reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                {
                    PropertyInfo propertyInfo = item.GetType().GetProperty(prop.Name);
                    propertyInfo.SetValue(item, Convert.ChangeType(reader.GetOrdinal(prop.Name), propertyInfo.PropertyType), null);
                }
            }
            return item;
        }
        protected virtual bool IsExistColumn(DataTable dt,string columnName)
        {
            dt.DefaultView.RowFilter = "ColumnName='" + columnName + "'";
            return (dt.DefaultView.Count > 0);
        }
    }
    /// <summary>
    /// Base class for custom repositories of the application.
    /// This is a shortcut of <see cref="UserManagementRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class UserManagementRepositoryBase<TEntity> : UserManagementRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        protected UserManagementRepositoryBase(IDbContextProvider<UserManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // Do not add any method here, add to the class above (since this inherits it)!!!
    }
    public abstract class SqlRepositoryBase<TEntity> : SqlRepositoryBase<TEntity,int>
        where TEntity : class, IEntity<int>, new()
    {
        protected SqlRepositoryBase(IDbContextProvider<UserManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}
