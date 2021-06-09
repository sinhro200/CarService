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
    public class OrderRepository : Repository<Order>, IRepository<Order>
    {
        public OrderRepository(DbContext context, ILogger log, Func<DbSet<Order>, IQueryable<Order>> includes = null) 
            : base(context, log, includes)
        {}

        public new List<Order> FindBy(Func<Order, bool> predicate)
        {
            return WithAllIncludes()
                .AsNoTracking<Order>()
                .Where(predicate)
                .OrderBy(o=>o.Car.Model.Title)
                .ToList<Order>();
        }

        public List<Order> FindByWithOrderingCreationDate(
            Func<Order, bool> predicate,
            bool isAsc
        )
        {
            IEnumerable<Order> ie = WithAllIncludes().AsNoTracking()
                .Where(predicate);
            if (isAsc)
                return ie.OrderBy(o => o.OpenDateTime == null)
                    .ThenBy(o => o.OpenDateTime)
                    .ThenBy(o=>o.CloseDateTime)
                    .ToList();
            else
                return ie.OrderByDescending(o => o.OpenDateTime == null)
                    .ThenByDescending(o => o.OpenDateTime)
                    .ThenBy(o => o.CloseDateTime)
                    .ToList();
        }

        public List<Order> FindByWithOrderingClosingDate(
            Func<Order, bool> predicate,
            bool isAsc
        )
        {
            IEnumerable<Order> ie = WithAllIncludes().AsNoTracking()
                .Where(predicate);
            if (isAsc)
                return ie.OrderBy(o => o.CloseDateTime == null)
                    .ThenBy(o => o.CloseDateTime)
                    .ThenBy(o=>o.OpenDateTime)
                    .ToList();
            else
                return ie.OrderByDescending(o => o.CloseDateTime == null)
                    .ThenByDescending(o => o.CloseDateTime)
                    .ThenBy(o => o.OpenDateTime)
                    .ToList();
        }

        public List<Order> FindByWithOrderingSumPrice(
            Func<Order, bool> predicate,
            bool isAsc
        )
        {
            IEnumerable<Order> ie = WithAllIncludes().AsNoTracking()
                .Where(predicate);
            if (isAsc)
                return ie.OrderBy(o => o.Services.Sum(s=>s.DefaultPrice))
                    .ThenBy(o => o.Car.ModelId)
                    .ThenBy(o => o.OpenDateTime)
                    .ThenBy(o => o.CloseDateTime)
                    .ToList();
            else
                return ie.OrderByDescending(o => o.Services.Sum(s => s.DefaultPrice))
                    .ThenBy(o => o.Car.ModelId)
                    .ThenBy(o => o.OpenDateTime)
                    .ThenBy(o => o.CloseDateTime)
                    .ToList();
        }

        public List<Order> FindByWithOrderingOwner(
            Func<Order, bool> predicate,
            bool isAsc
        )
        {
            IEnumerable<Order> ie = WithAllIncludes().AsNoTracking()
                .Where(predicate);
            if (isAsc)
                return ie.OrderBy(o => o.Car.Owner.Name)
                    .ThenBy(o => o.Car.ModelId)
                    .ThenBy(o => o.OpenDateTime)
                    .ThenBy(o => o.CloseDateTime)
                    .ToList();
            else
                return ie.OrderByDescending(o => o.Car.Owner.Name)
                    .ThenBy(o => o.Car.ModelId)
                    .ThenBy(o => o.OpenDateTime)
                    .ThenBy(o => o.CloseDateTime)
                    .ToList();
        }

        public List<Order> FindByWithOrderingCar(
            Func<Order, bool> predicate,
            bool isAsc
        )
        {
            IEnumerable<Order> ie = WithAllIncludes().AsNoTracking()
                .Where(predicate);
            if (isAsc)
                return ie.OrderBy(o => o.Car.Model.Title)
                    .ThenBy(o => o.Car.Model.Brand.Title)
                    .ThenBy(o => o.OpenDateTime)
                    .ThenBy(o => o.CloseDateTime)
                    .ToList();
            else
                return ie.OrderByDescending(o => o.Car.Model.Title)
                    .ThenByDescending(o => o.Car.Model.Brand.Title)
                    .ThenBy(o => o.OpenDateTime)
                    .ThenBy(o => o.CloseDateTime)
                    .ToList();
        }
    }
}
