using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using TAiMStore.Domain;
using TAiMStore.Model.Factory;

namespace TAiMStore.Model.Repository
{
    public abstract class RepositoryBase<TEntity> where TEntity : Entity
    {
        private StoreContext _dataContext;
        protected readonly IDbSet<TEntity> DbSet;

        public IFactory DatabaseFactory { get; private set; }

        protected RepositoryBase(IFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            DbSet = DataContext.Set<TEntity>();
        }

        protected StoreContext DataContext
        {
            get { return _dataContext ?? (_dataContext = DatabaseFactory.Get()); }
        }

        public virtual void Add(TEntity entity)
        {
            DbSet.Add(entity);
            _dataContext.Entry(entity).State = EntityState.Added;
        }

        public void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;

        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            var objects = DbSet.Where(where).AsQueryable();
            foreach (var obj in objects)
            {
                DbSet.Remove(obj);
            }
        }

        public int GetCount()
        {
            return DbSet.Count();
        }

        public int GetCount(Expression<Func<TEntity, bool>> where)
        {
            var objects = DbSet.Where(where).AsQueryable();

            return objects.Count();
        }

        public bool Any(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.Any(where);
        }

        public TEntity GetById(long id)
        {
            return DbSet.Find(id);
        }

        public TEntity GetById(string id)
        {
            return DbSet.Find(id);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.FirstOrDefault(where);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.Where(where).ToList();
        }
    }

    public class Pagination
    {
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
