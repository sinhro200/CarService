using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class UnitOfWork : IDisposable
    {
        private DbContext _context;

        private IRepository<User> _userRepository;
        private IRepository<Car> _carRepository;
        private IRepository<Model> _modelRepository;
        private IRepository<Brand> _brandRepository;
        private IRepository<Mechanic> _mechanicRepository;
        private IRepository<Service> _serviceRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<OrderServiceStatus> _statusRepository;

        
        public UnitOfWork(string connectionString)
        {
            _context = new ApplicationContext(connectionString);
        }

        public IRepository<User> Users
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new Repository<User>(_context,
                        ds => {
                            return ds.Include(u=>u.Cars);
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
                        ds => {
                            return ds.Include(c => c.Model)
                              .Include(c => c.Owner);
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
                        ds => {
                            ds.Include(m => m.Brand);
                            ds.Include(m => m.Cars);
                            return ds;
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
                    _brandRepository = new Repository<Brand>(_context);
                return _brandRepository;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new Repository<Order>(_context);
                return _orderRepository;
            }
        }

        public IRepository<Mechanic> Mechanics
        {
            get
            {
                if (_mechanicRepository == null)
                    _mechanicRepository = new Repository<Mechanic>(_context);
                return _mechanicRepository;
            }
        }

        public IRepository<OrderServiceStatus> Statuses
        {
            get
            {
                if (_statusRepository == null)
                    _statusRepository = new Repository<OrderServiceStatus>(_context);
                return _statusRepository;
            }
        }

        public IRepository<Service> Services
        {
            get
            {
                if (_serviceRepository == null)
                    _serviceRepository = new Repository<Service>(_context);
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
