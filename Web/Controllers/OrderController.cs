﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using Business.Interfaces;
using Business.DTO;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        

        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            this.orderService = orderService;
        }

        [HttpGet]
        [Route("byCar")]
        public List<OrderDto> AllForCar(int carId)
        {
            _logger.LogInformation("Got req to get all orders for carId " + carId);
            return orderService.AllOrdersForCar(carId);
        }

        [HttpGet]
        [Route("servicesByOrder")]
        public List<FullServiceDto> ServicesForOrder(int orderId)
        {
            _logger.LogInformation("Got req to get all service info for orderId " + orderId);
            return orderService.ServicesForOrder(orderId);
        }

        [HttpGet]
        [Route("all")]
        public List<OrderDto> All()
        {
            _logger.LogInformation("Got req to get all orders ");
            return orderService.getAllItems();
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(OrderDto dto)
        {
            _logger.LogInformation($"Got req to add order {dto}");
            OrderDto addedDto = orderService.addItemReturning(dto);
            return addedDto == null ? NotFound() : Ok(addedDto);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"Got req to get order by id {id}");
            OrderDto dto = orderService.getItem(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPut]
        [Route("edit")]
        public IActionResult Edit(OrderDto dto)
        {
            _logger.LogInformation($"Got req to edit order {dto}");
            OrderDto res = orderService.editItemReturning(dto);
            return Ok(res);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Got req to delete order with id {id}");
            orderService.deleteItem(id);
            return Ok();
        }
    }
}
