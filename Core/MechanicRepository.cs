using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class MechanicRepository : Repository<Mechanic>, IRepository<Mechanic>
    {
        public MechanicRepository(DbContext context, ILogger log, Func<DbSet<Mechanic>, IQueryable<Mechanic>> includes = null) 
            : base(context, log, includes)
        {}

        public new Mechanic Update(Mechanic mechanic)
        {
            log.LogInformation("Updating " + mechanic.ToString());
            Mechanic existing = _dbSet.Include(m=>m.Services)
                .SingleOrDefault(m=>m.Id==mechanic.Id);
            existing.Services.Clear();
            existing.Services = mechanic.Services;
            if (mechanic.Name != null)
                existing.Name = mechanic.Name;
            if (mechanic.Cars != null)
                existing.Cars = mechanic.Cars;
            _dbSet.Update(existing);
            _dbContext.SaveChanges();
            _dbContext.Entry(existing).State = EntityState.Detached;
            _dbContext.SaveChanges();
            log.LogInformation("Successfully updated " + mechanic.ToString());
            return mechanic;
        }

        public Mechanic SingleOrNullForServices(Func<Mechanic,bool> predicate)
        {
            log.LogInformation("Single or null by %predicate");
            try
            {
                return _dbSet.Include(m=>m.Services)
                        .ThenInclude(s=>s.OrderServices)
                        .ThenInclude(os=>os.Mechanic)
                    .Include(m=>m.Services)
                        .ThenInclude(s=>s.OrderServices)
                        .ThenInclude(os=>os.OrderServiceStatus)
                    .Include(m => m.Services)
                        .ThenInclude(s => s.OrderServices)
                        .ThenInclude(os=>os.Order)
                        .ThenInclude(o=>o.Car)
                        .ThenInclude(c=>c.Model)
                        .ThenInclude(c => c.Brand)
                    .Include(m => m.Services)
                        .ThenInclude(s => s.OrderServices)
                        .ThenInclude(os => os.Order)
                        .ThenInclude(o => o.Car)
                        .ThenInclude(c=>c.Owner)
                    .Single(predicate);
            }
            catch (Exception e)
            {
                log.LogCritical(e, "Exc in single or null");
                return null;
            }
        }
    }
}
