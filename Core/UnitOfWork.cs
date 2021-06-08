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
    public class UnitOfWork : IDisposable
    {
        private readonly LoggerFactory loggerFactory;
        private DbContext _context;

        private IRepository<User> _userRepository;
        private IRepository<Car> _carRepository;
        private IRepository<Model> _modelRepository;
        private IRepository<Brand> _brandRepository;
        private IRepository<Mechanic> _mechanicRepository;
        private IRepository<Service> _serviceRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<OrderServiceStatus> _statusRepository;

        
        public UnitOfWork(LoggerFactory loggerFactory)
        {
            _context = new ApplicationContext("Host=localhost;Port=5432;Database=carservice;Username=carservice;Password=1234;ENCODING=UTF8");
            this.loggerFactory = loggerFactory;
        }

        public IRepository<User> Users
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new Repository<User>(_context,
                        loggerFactory.CreateLogger<IRepository<User>>(),
                        ds => {
                            return ds.Include(u=>u.Cars)
                            .ThenInclude(c=>c.Model)
                            .ThenInclude(m=>m.Brand).AsNoTracking()
                            
                            ;
                        }
                        );
                return _userRepository;
            }
        }

        public IRepository<Car> Cars
        {
            get
            {
                if (_carRepository == null)
                    _carRepository = new Repository<Car>(_context,
                        loggerFactory.CreateLogger("Repository<Car>"),
                        ds => {
                            return ds.Include(c => c.Owner)
                                //.ThenInclude(u=>u.Cars)
                                //.ThenInclude(c=>c.Model)
                                //.ThenInclude(m=>m.Brand).AsNoTracking()
                                .Include(c => c.Model)
                                .ThenInclude(m=>m.Brand).AsNoTracking();
                        }
                        );
                return _carRepository;
            }
        }

        public IRepository<Model> Models
        {
            get
            {
                if (_modelRepository == null)
                    _modelRepository = new Repository<Model>(_context,
                        loggerFactory.CreateLogger("Repository<Model>"),
                        ds => {
                            return ds.Include(m => m.Brand).AsNoTracking()
                                    .Include(m => m.Cars).AsNoTracking();
                            
                        }
                        );
                return _modelRepository;
            }
        }

        public IRepository<Brand> Brands
        {
            get
            {
                if (_brandRepository == null)
                    _brandRepository = new Repository<Brand>(_context,
                        loggerFactory.CreateLogger("Repository<Brand>"),
                        ds =>
                        {
                            return ds.Include(b=>b.Models).AsNoTracking();
                        }
                        );
                return _brandRepository;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new Repository<Order>(_context,
                        loggerFactory.CreateLogger("Repository<Order>"),
                        ds =>
                        {
                            return ds.Include(o => o.OrderServices)
                                    .ThenInclude(os=>os.Mechanic)
                                    .Include(o => o.OrderServices)
                                    .ThenInclude(os=>os.OrderServiceStatus)
                                    .Include(o => o.OrderServices)
                                    .ThenInclude(os => os.Service)
                                    .Include(o=>o.Services)
                                    .ThenInclude(s=>s.Mechanics)
                                    .Include(o=>o.Car)
                                    .ThenInclude(c=>c.Model)
                                    .ThenInclude(m=>m.Brand)
                                    .Include(o => o.Car)
                                    .ThenInclude(c=>c.Owner)
                                    ;
                        });
                return _orderRepository;
            }
        }

        public IRepository<Mechanic> Mechanics
        {
            get
            {
                if (_mechanicRepository == null)
                    _mechanicRepository = new MechanicRepository(_context,
                        loggerFactory.CreateLogger("Repository<Mechanic>"),
                        ds =>
                        {
                            return ds.Include(m => m.Cars)
                                    .ThenInclude(c => c.Model)
                                    .ThenInclude(m => m.Brand).AsNoTracking()
                                    .Include(m => m.Services).AsNoTracking()
                                    ;
                        });
                return _mechanicRepository;
            }
        }

        public IRepository<OrderServiceStatus> Statuses
        {
            get
            {
                if (_statusRepository == null)
                    _statusRepository = new Repository<OrderServiceStatus>(_context,
                        loggerFactory.CreateLogger("Repository<OrderServiceStatus>"),
                        ds =>
                        {
                            return ds.Include(oss => oss.OrderServices).AsNoTracking()
                            ;
                        });
                return _statusRepository;
            }
        }

        public IRepository<Service> Services
        {
            get
            {
                if (_serviceRepository == null)
                    _serviceRepository = new Repository<Service>(_context,
                        loggerFactory.CreateLogger("Repository<Service>"),
                        ds =>
                        {
                            return ds.Include(s => s.Mechanics).AsNoTracking()
                            .Include(s => s.Orders).AsNoTracking()
                            .Include(s => s.OrderServices).AsNoTracking();
                        });
                return _serviceRepository;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //private static IRepository<T> GET_WITH_CREATE<T>(out IRepository<T> repo, DbContext context)
        //    where T : class
        //{
        //    if (repo == null)
        //    {
        //        repo = new Repository<T>(context);
        //    }

        //    return repo;
        //}
    }
}
