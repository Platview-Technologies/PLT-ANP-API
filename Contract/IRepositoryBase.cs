using Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace Contract
{
    public interface IRepositoryBase<T, R> where T : class, IEntityBase<R>, new()
    {
        /// <summary>
        /// This return all IQueryable records including those marks as deleted
        /// </summary>
        /// <returns></returns>
        IQueryable<T> FindAll(bool trackChanges);
        /// <summary>
        /// This return IQueryable records that are not marks as deleted
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expresion,
            bool trackChanges);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
