using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using Business.Interfaces;
using Business.DTO;
using Core;
using Business.Services;
using Core.Entities;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {


        private readonly ILogger<TestController> _logger;
        private readonly UnitOfWork unitOfWork;

        public TestController(ILogger<TestController> logger, UnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("test")]
        public IActionResult Test(OrderDto dto)
        {
            Order order = mapToModel(dto);
            unitOfWork.Orders.Save(order);

            order = unitOfWork.Orders.SingleOrNull(it=>it.Id == order.Id);

            List<Core.Entities.OrderService> orderServices = new List<Core.Entities.OrderService>();
            foreach (FullServiceDto s in dto.Services)
            {
                Service serv = unitOfWork.Services.SingleOrNull(serv => serv.Id == s.ServiceId);
                Core.Entities.OrderService os = new Core.Entities.OrderService
                {
                    OrderId = order.Id,
                    ServiceId = serv.Id,
                    Price = serv == null ? -1 : serv.DefaultPrice,
                    OrderServiceStatusId = s.Status == null ? 1 : s.Status.Id == 0 ? 1 : s.Status.Id
                };
                orderServices.Add(os);
            };
            order.OrderServices = orderServices;
            unitOfWork.Orders.Update(order);

            Order withIncludes = unitOfWork.Orders.SingleOrNull(m => m.Id == order.Id);
            
            OrderDto res = withIncludes == null ? null : mapToDto(withIncludes);
            return Ok(res);
        }

        private Order mapToModel(OrderDto dto)
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
                    Service service = unitOfWork.Services.SingleOrNull(serv => serv.Id == s.ServiceId);
                    if (service == null)
                    {
                        throw new System.NullReferenceException("Service is null while get from repo while mapping from dto");
                    }
                    return service;
                }
                )

            };
            return o;
        }

        private OrderDto mapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CreateDateTime = order.OpenDateTime,
                Services = order.OrderServices.ConvertAll(s => {
                    return new FullServiceDto
                    {
                        ServiceId = s.ServiceId,
                        Price = s.Price,
                        Title = s.Service.Title,
                        Status = new FullServiceStatusDto
                        {
                            Id = s.OrderServiceStatus.Id,
                            Title = s.OrderServiceStatus.Title
                        },
                        Mechanic = s.Mechanic == null ? null : new MechanicDto
                        {
                            Id = s.Mechanic.Id,
                            Name = s.Mechanic.Name
                        }
                    };
                }),
                Car = new CarDto
                {
                    Id = order.Car.Id,
                    Model = new ModelDto
                    {
                        Id = order.Car.Model.Id,
                        BrandDto = new BrandDto
                        {
                            Id = order.Car.Model.Brand.Id,
                            Title = order.Car.Model.Brand.Title
                        },
                        Title = order.Car.Model.Title
                    },
                    Owner = new UserDto
                    {
                        Id = order.Car.OwnerId,
                        Name = order.Car.Owner.Name
                    }
                }
            };
        }
    }
}
