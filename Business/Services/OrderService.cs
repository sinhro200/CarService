using Business.DTO;
using Business.Interfaces;
using Core;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Business.Services
{
    public class OrderService : DefaultService<OrderDto, Order, OrderService>, IOrderService
    {

        private readonly IRepository<Car> carRepository;
        private readonly IRepository<Service> serviceRepository;
        private readonly IRepository<User> userRepository;

        public OrderService(ILogger<OrderService> logger, UnitOfWork repos)
            : base(repos.Orders, logger,
                  dto =>
                  {
                  Order o = new Order
                  {
                      Id = dto.Id,
                      CarId = dto.Car.Id,
                      OpenDateTime = dto.CreateDateTime,
                      OrderServices = new List<Core.Entities.OrderService>(),
                      //OrderServices = dto.Services.ConvertAll(s =>
                      //{
                      //    Service serv = repos.Services.SingleOrNull(serv => serv.Id == s.ServiceId);
                      //    Core.Entities.OrderService os = new Core.Entities.OrderService
                      //    {
                      //        //проблема в том, что dto.Id ещё нету
                      //        OrderId = dto.Id,
                      //        ServiceId = s.ServiceId,
                      //        Price = serv == null ? -1 : serv.DefaultPrice,
                      //        OrderServiceStatusId = s.Status == null ? 1 : s.Status.Id == 0 ? 1 : s.Status.Id

                      //    };
                      //    return os;
                      //}),
                      Services = dto.Services.ConvertAll(s =>
                          {

                              Service service = repos.Services.FindByIdWithoutIncludes(s.ServiceId);
                              //Service service = repos.Services.SingleOrNull(serv => serv.Id == s.ServiceId);
                              //if(service == null)
                              //{
                              //    throw new System.NullReferenceException("Service is null while get from repo while mapping from dto");
                              //}
                              return service;
                          }
                      )
                  
                      };
                      return o;
                  },
                  order =>
                  {
                      return new OrderDto {
                          Id = order.Id,
                          CreateDateTime = order.OpenDateTime,
                          ClosedDateTime = order.CloseDateTime,
                          IsClosed = order.IsClosed,
                          Services = order.OrderServices.ConvertAll(s => {
                              return new FullServiceDto { 
                                  ServiceId = s.ServiceId,
                                  Price = s.Price,
                                  Title = s.Service.Title,
                                  Status = new FullServiceStatusDto{
                                      Id = s.OrderServiceStatus.Id,
                                      Title = s.OrderServiceStatus.Title
                                  },
                                  Mechanic = s.Mechanic == null ? null : new UserDto
                                  {
                                      Id = s.Mechanic.Id,
                                      Name = s.Mechanic.Name
                                  }
                              };
                          }),
                          Car = new CarDto { 
                              Id = order.Car.Id, 
                              Model = new ModelDto { 
                                  Id = order.Car.Model.Id,
                                  BrandDto = new BrandDto { 
                                      Id = order.Car.Model.Brand.Id,
                                      Title = order.Car.Model.Brand.Title
                                  },
                                  Title = order.Car.Model.Title
                              }, 
                              Owner = new UserDto {
                                  Id = order.Car.OwnerId,
                                  Name = order.Car.Owner.Name
                              }
                          }
                      };
                  },
                  brand => brand.Id
                  )
        {
            this.carRepository = repos.Cars;
            this.serviceRepository = repos.Services;
            this.userRepository = repos.Users;
        }

        public new OrderDto addItemReturning(OrderDto dto)
        {
            if (dto.Id <= 0)
            {
                dto.CreateDateTime = System.DateTime.Now;
                logger.LogDebug("Converting DTO: " + dto + "; to Model. ");
                Order order = dtoToModelConverter.Invoke(dto);
                logger.LogDebug("Converted to model: " + order + ". Then saving in repo...");
                Order savedNotFilled = repository.Save(order);
                logger.LogDebug("Saved in repo. Got not filled:" + savedNotFilled);
                foreach (Core.Entities.OrderService os in order.OrderServices)
                {
                    Service relatedService = os.Service;
                    os.Price = relatedService == null ? -1 : relatedService.DefaultPrice;
                    FullServiceStatusDto status = dto.Services.Find(s => s.ServiceId == relatedService.Id).Status;
                    os.OrderServiceStatusId = status == null ? 1 : status.Id == 0 ? 1 : status.Id;
                }
                repository.Update(order);

                Order withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == idGetter.Invoke(order));
                logger.LogDebug("Got with includes from repo:" + withIncludes + ". Returning...");
                OrderDto res = withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
                return res;
                //List<Core.Entities.OrderService> orderServices = new List<Core.Entities.OrderService>();
                //logger.LogDebug("Creating OrderServices...");
                //foreach (FullServiceDto s in dto.Services)
                //{
                //    //Service serv = serviceRepository.SingleOrNull(serv => serv.Id == s.ServiceId);
                //    Service serv = serviceRepository.FindByIdWithoutIncludes(s.ServiceId);
                //    Core.Entities.OrderService os = new Core.Entities.OrderService
                //    {
                //        OrderId = savedNotFilled.Id,
                //        ServiceId = serv.Id,
                //        Price = serv == null ? -1 : serv.DefaultPrice,
                //        OrderServiceStatusId = s.Status == null ? 1 : s.Status.Id == 0 ? 1 : s.Status.Id
                //    };
                //    orderServices.Add(os);
                //};
                //savedNotFilled.OrderServices = orderServices;
                //logger.LogDebug("Updating Order with created OrderServices...");
                //savedNotFilled = repository.Update(savedNotFilled);
                //logger.LogDebug("Updated successfully");
                //Order withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == idGetter.Invoke(savedNotFilled));
                //logger.LogDebug("Got with includes from repo:" + withIncludes + ". Returning...");
                //OrderDto res = withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
                //return res;
            }
            return null;
        }

        public new void addItem(OrderDto dto)
        {
            if (dto.Id <= 0)
            {
                dto.CreateDateTime = System.DateTime.Now;
                logger.LogDebug("Converting DTO: " + dto + "; to Model. ");
                Order order = dtoToModelConverter.Invoke(dto);
                logger.LogDebug("Converted to model: " + order + ". Then saving in repo...");
                Order savedNotFilled = repository.Save(order);
                logger.LogDebug("Saved in repo. Got not filled:" + savedNotFilled);
                foreach (Core.Entities.OrderService os in order.OrderServices)
                {
                    Service relatedService = os.Service;
                    os.Price = relatedService == null ? -1 : relatedService.DefaultPrice;
                    FullServiceStatusDto status = dto.Services.Find(s => s.ServiceId == relatedService.Id).Status;
                    os.OrderServiceStatusId = status == null ? 1 : status.Id == 0 ? 1 : status.Id;
                }
                repository.Update(order);
            }
        }

        public List<OrderDto> AllOrdersForCar(int carId)
        {
            if (carRepository.SingleOrNull(c=>c.Id==carId) != null)
                return repository.FindBy(o => o.CarId == carId).ConvertAll(modelToDtoConverter.Invoke);
            return new List<OrderDto>();
        }

        public List<FullServiceDto> ServicesForOrder(int orderId)
        {
            Order o = repository.SingleOrNull(o => o.Id == orderId);
            if (o == null)
                return new List<FullServiceDto>();

            return o.OrderServices.ConvertAll(os =>
            {
                User mechanicUser = userRepository.SingleOrNull(u => u.Id == os.MechanicId);
                MechanicDto mech = mechanicUser==null ? null : 
                    new MechanicDto { Id = mechanicUser.Id, Name = mechanicUser.Name };
                return new FullServiceDto
                {
                    ServiceId = os.ServiceId,
                    Price = os.Price,
                    Title = os.Service.Title,
                    Mechanic = mech,
                    Status = new FullServiceStatusDto { Id = os.OrderServiceStatus.Id, Title = os.OrderServiceStatus.Title },
                };
            });
        }
    }
}
