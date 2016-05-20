using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TAiMStore.Model.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> where);
        int GetCount();
        int GetCount(Expression<Func<TEntity, bool>> where);
        bool Any(Expression<Func<TEntity, bool>> where);
        TEntity GetById(long id);
        TEntity GetById(string id);
        TEntity Get(Expression<Func<TEntity, bool>> where);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where);
    }
}
