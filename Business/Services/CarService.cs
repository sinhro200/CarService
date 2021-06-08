using Business.DTO;
using Business.Interfaces;
using Core;
using Core.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Business.Services
{
    public class CarService : DefaultService<CarDto, Car, CarService>, ICarService
    {

        public CarService(ILogger<CarService> logger, UnitOfWork repos)
            : base(repos.Cars, logger,
                  dto =>
                  {
                      return new Car { Id = dto.Id, ModelId = dto.Model.Id, OwnerId = dto.Owner.Id };
                  },
                  car =>
                  {
                      return new CarDto
                      {
                          Id = car.Id,
                          Owner = new UserDto { Id = car.OwnerId, Name = car.Owner.Name },
                          Model = new ModelDto { Id = car.Model.Id, Title = car.Model.Title, 
                              BrandDto = new BrandDto { Id = car.Model.Brand.Id, Title = car.Model.Brand.Title  }
                          }
                      };
                  },
                  brand => brand.Id
                  )
        { }

        public List<CarDto> GetByUser(int userId)
        {
            List<Car> carsInDb = repository.FindBy(c => c.OwnerId == userId);
            return carsInDb.ConvertAll(modelToDtoConverter.Invoke);
        }
    }
}
