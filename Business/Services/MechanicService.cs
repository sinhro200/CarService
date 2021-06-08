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
    public class MechanicService : DefaultService<MechanicDto,Mechanic,MechanicService>, IMechanicService
    {

        public MechanicService(ILogger<MechanicService> logger, UnitOfWork repos) 
            : base(repos.Mechanics,logger,
                  dto =>
                  {
                      //Mechanic existing = repos.Mechanics.FindByIdWithoutIncludes(dto.Id);
                      //if (existing == null)
                      //{ 
                          return new Mechanic {
                              Id = dto.Id,
                              Name = dto.Name,
                              Cars = dto.Cars == null ? null : dto.Cars.ConvertAll(cdto =>
                              {
                                  return new Car { Id = cdto.Id, ModelId = cdto.Model.Id, OwnerId = dto.Id };
                              }),
                              Services = dto.Services.ConvertAll(s =>
                              {
                                  Service service = repos.Services.FindByIdWithoutIncludes(s.ServiceId);
                                  //Service service = repos.Services.SingleOrNull(ser => ser.Id == s.ServiceId);
                                  //if (service == null)
                                  //{
                                  //    throw new System.NullReferenceException("Service is null while get from repo while mapping from dto");
                                  //}
                                  return service;
                              })
                          };
                      //}
                      //else
                      //{
                      //    existing.Name = dto.Name;
                      //    existing.Cars = dto.Cars == null ? existing.Cars : dto.Cars.ConvertAll(cdto =>
                      //    {
                      //        return new Car { Id = cdto.Id, ModelId = cdto.Model.Id, OwnerId = dto.Id };
                      //    });
                      //    List<Service> newServices = dto.Services.ConvertAll(s =>
                      //    {
                      //        Service service = repos.Services.FindByIdWithoutIncludes(s.ServiceId);
                      //        //Service service = repos.Services.SingleOrNull(ser => ser.Id == s.ServiceId);
                      //        //if (service == null)
                      //        //{
                      //        //    throw new System.NullReferenceException("Service is null while get from repo while mapping from dto");
                      //        //}
                      //        return service;
                      //    });
                      //    List<Service> newList = new List<Service>();
                      //    foreach(Service newServ in newServices)
                      //    {
                      //        Service foundExisting = existing.Services.FirstOrDefault(ex => ex.Id == newServ.Id);
                      //       if (foundExisting == null || foundExisting.Equals(default(Service)))
                      //        {
                      //            //ещё нету в бд
                      //            newList.Add(newServ);
                      //        }
                      //        else
                      //        {
                      //            newList.Add(foundExisting);
                      //        }
                      //    }
                      //    existing.Services = newList;
                      //    return existing;
                      //}
                  },
                  model =>
                  {
                      MechanicDto user = new MechanicDto { Id = model.Id, Name = model.Name };
                      List<CarDto> userCars = model.Cars == null ? null : model.Cars.ConvertAll(c =>
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
                      List<ServiceDto> mechanicServices = model.Services.ConvertAll(s =>
                      {
                          return new ServiceDto
                          {
                              ServiceId = s.Id,
                              Title = s.Title,
                              Price = s.DefaultPrice
                          };
                      });
                      MechanicDto mechanicNew = new MechanicDto{ 
                          Id = model.Id, Name = model.Name, 
                          Cars = userCars, Services = mechanicServices };
                      return mechanicNew;
                  },
                  user => user.Id
                  )
        {}

        public new MechanicDto editItemReturning(MechanicDto itemDto)
        {
            Mechanic newModel = dtoToModelConverter.Invoke(itemDto);
            if (repository.Contains(newModel))
            {
                repository.Update(newModel);
                return modelToDtoConverter.Invoke(newModel);
                //Mechanic withoutIncludes = repository.Update(model);
                //Mechanic withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == idGetter.Invoke(withoutIncludes));
                //return withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
            }

            return null;
        }
    }
}
