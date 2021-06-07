using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Repository<T> : IRepository<T>
        where T : class
    {

        protected DbContext _dbContext;

        protected DbSet<T> _dbSet;
        protected Func<DbSet<T>, IQueryable<T>> includes = null;

        public Repository(DbContext context, Func<DbSet<T>, IQueryable<T>> includes = null)
        {
            _dbContext = context;
            this.includes = includes;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> WithAllIncludes()
        {
            if (includes == null)
                return _dbSet;
            return includes.Invoke(_dbSet);
        }


        public T Delete(T item)
        {
            _dbSet.Remove(item);
            _dbContext.SaveChanges();
            return item;
        }

        public T Delete(int id)
        {
            T found = _dbSet.Find(id);
            if (found != null)
            {
                return Delete(found);
            }
            return null;
        }

        public List<T> FindAll()
        {
            return WithAllIncludes().AsNoTracking().ToList<T>();
        }

        public List<T> FindBy(Func<T, bool> predicate)
        {
            return WithAllIncludes().AsNoTracking<T>().Where(predicate).ToList<T>();
        }

        public T SingleOrNull(Func<T, bool> predicate)
        {
            try{
                //return WithAllIncludes().AsNoTracking<T>().Single(predicate);
                return WithAllIncludes().Single(predicate);
            } catch(Exception e)
            {
                Console.WriteLine("Exc in repo." + e);
                return null;
            }
        }

        public T FindByIdWithoutIncludes(int id)
        {
            return _dbSet.Find(id);
        }


        public T Save(T item)
        {
            T ent = _dbSet.Add(item).Entity;
            _dbContext.SaveChanges();
            return ent;
        }

        public void SaveAll(IEnumerable<T> items)
        {
            _dbSet.AddRange(items);
            _dbContext.SaveChanges();
        }

        public T Update(T item)
        {
            _dbSet.Update(item);
            _dbContext.SaveChanges();
            return item;
        }

        public bool Contains(T item)
        {
            return _dbSet.Contains<T>(item);
        }

    }
}
