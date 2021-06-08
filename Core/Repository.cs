using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
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

        protected ILogger log;

        protected DbContext _dbContext;

        protected DbSet<T> _dbSet;
        protected Func<DbSet<T>, IQueryable<T>> includes = null;

        public Repository(DbContext context, ILogger log,  Func<DbSet<T>, IQueryable<T>> includes = null)
        {
            this.log = log;
            _dbContext = context;
            this.includes = includes;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> WithAllIncludes()
        {
            log.LogInformation("With includes");
            if (includes == null)
                return _dbSet;
            return includes.Invoke(_dbSet);
        }


        public T Delete(T item)
        {
            log.LogInformation("Deleting item " + item.ToString());
            _dbSet.Remove(item);
            _dbContext.SaveChanges();
            log.LogInformation("Deleted succesfully ");
            return item;
        }

        public T Delete(int id)
        {
            log.LogInformation("Deleting by id " + id);
            T found = _dbSet.Find(id);
            if (found != null)
            {
                T res = Delete(found);
                log.LogInformation("Deleted successfully");
                return res;
            }
            log.LogInformation("Not found by id " + id);
            return null;
        }

        public List<T> FindAll()
        {
            log.LogInformation("Find all ");
            //return WithAllIncludes().AsNoTracking().ToList<T>();
            return WithAllIncludes().ToList<T>();
        }

        public List<T> FindBy(Func<T, bool> predicate)
        {
            log.LogInformation("FindBy %predicate");
            return WithAllIncludes().AsNoTracking<T>().Where(predicate).ToList<T>();
        }

        public T SingleOrNull(Func<T, bool> predicate)
        {
            log.LogInformation("Single or null by %predicate");
            try
            {
                //return WithAllIncludes().AsNoTracking<T>().Single(predicate);
                return WithAllIncludes().Single(predicate);
            } catch(Exception e)
            {
                log.LogCritical(e,"Exc in single or null");
                return null;
            }
        }

        public T FindByIdWithoutIncludes(int id)
        {
            return _dbSet.Find(id);
        }


        public T Save(T item)
        {
            log.LogInformation("Saving " + item.ToString());
            //EntityEntry<T> res = _dbSet.Add(item);
            _dbSet.Add(item);
            _dbContext.SaveChanges();
            _dbContext.Entry(item).State = EntityState.Detached;
            _dbContext.SaveChanges();
            _dbContext.ChangeTracker.Clear();
            _dbContext.SaveChanges();
            //T ent = res.Entity;
            //res.State = EntityState.Detached;
            //_dbContext.Entry(ent).State = EntityState.Detached;
            //_dbContext.Entry(item).State = EntityState.Detached;
            //_dbContext.SaveChanges();
            log.LogInformation("Successfully saved " + item.ToString());
            return item;
        }

        public void SaveAll(IEnumerable<T> items)
        {
            _dbSet.AddRange(items);
            _dbContext.SaveChanges();
        }

        public T Update(T item)
        {
            log.LogInformation("Updating " + item.ToString());
            _dbSet.Update(item);
            //_dbContext.Entry(item).State = EntityState.Modified;
            _dbContext.SaveChanges();
            _dbContext.Entry(item).State = EntityState.Detached;
            _dbContext.SaveChanges();
            _dbContext.ChangeTracker.Clear();
            _dbContext.SaveChanges();
            //EntityEntry<T> res = _dbSet.Update(item);
            //_dbContext.SaveChanges();
            //res.State = EntityState.Detached;
            //T item = res.Entity;
            log.LogInformation("Successfully updated " + item.ToString());
            return item;
        }

        public bool Contains(T item)
        {
            return _dbSet.Contains<T>(item);
        }

    }
}
