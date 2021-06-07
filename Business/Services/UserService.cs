using Business.DTO;
using Business.Interfaces;
using Core;
using Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserService : DefaultService<UserDto,User,UserService>, IUserService
    {

        public UserService(ILogger<UserService> logger, UnitOfWork repos) 
            : base(repos.Users,logger,
                  dto =>
                  {
                      return new User { 
                          Id = dto.Id, 
                          Name = dto.Name,
                          Cars = dto.Cars == null ? null : dto.Cars.ConvertAll(cdto =>
                          {
                              return new Car { Id = cdto.Id, ModelId = cdto.Model.Id, OwnerId = dto.Id };
                          })
                      };
                  },
                  model =>
                  {
                      UserDto user = new UserDto { Id = model.Id, Name = model.Name };
                      List<CarDto> userCars = model.Cars.ConvertAll(c =>
                      {
                          CarDto carDto = new CarDto
                          {
                              Id = c.Id,
                              Model = new ModelDto
                              {
                                  Id = c.Model.Id,
                                  BrandDto = new BrandDto
                                  {
                                      Id = c.Model.Brand.Id,
                                      Title = c.Model.Brand.Title
                                  },
                                  Title = c.Model.Title
                              },
                              Owner = user
                          };
                          return carDto;
                      });
                      UserDto userNew = new UserDto { Id = model.Id, Name = model.Name, Cars = userCars };
                      return userNew;
                  },
                  user => user.Id
                  )
        {}
    }
}
