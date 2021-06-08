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
    class MechanicRepository : Repository<Mechanic>, IRepository<Mechanic>
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
    }
}
