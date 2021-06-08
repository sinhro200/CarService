using Business.DTO;
using Business.Interfaces;
using Core;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Business.Services
{
    public class ServiceService : DefaultService<ServiceDto, Service, ServiceService>, IServiceService
    {
        private readonly IRepository<Car> carRepository;
        public ServiceService(ILogger<ServiceService> logger, UnitOfWork repos)
            : base(repos.Services, logger,
                  dto =>
                  {
                      return new Service { Id = dto.ServiceId, Title = dto.Title, DefaultPrice = dto.Price };
                  },
                  service =>
                  {
                      return new ServiceDto { ServiceId = service.Id, Title = service.Title, Price = service.DefaultPrice };
                  },
                  service => service.Id
                  )
        {
            this.carRepository = repos.Cars;
        }

        public List<ServiceDto> AllServicesForCar(int carId)
        {
            if(carRepository.SingleOrNull(c=> c.Id == carId) != null)
                return getAllItems();
            return new List<ServiceDto>();
        }
    }
}
