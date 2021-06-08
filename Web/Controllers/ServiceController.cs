using Microsoft.AspNetCore.Mvc;
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
    public class ServiceController : ControllerBase
    {
        
        private readonly ILogger<ServiceController> _logger;
        private readonly IServiceService serviceService;

        public ServiceController(ILogger<ServiceController> logger, IServiceService serviceService)
        {
            _logger = logger;
            this.serviceService = serviceService;
        }

        [HttpGet]
        [Route("byCar")]
        public List<ServiceDto> All(int carId)
        {
            _logger.LogInformation("Got req to get all services for carId " + carId);
            return serviceService.AllServicesForCar(carId);
        }

        [HttpGet]
        [Route("all")]
        public List<ServiceDto> All()
        {
            _logger.LogInformation("Got req to get all services");
            return serviceService.getAllItems();
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(ServiceDto dto)
        {
            _logger.LogInformation($"Got req to add service {dto}");
            ServiceDto addedDto = serviceService.addItemReturning(dto);
            return addedDto == null ? NotFound() : Ok(addedDto);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"Got req to get service by id {id}");
            ServiceDto dto = serviceService.getItem(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPut]
        [Route("edit")]
        public IActionResult Edit(ServiceDto dto)
        {
            _logger.LogInformation($"Got req to edit service {dto}");
            ServiceDto res = serviceService.editItemReturning(dto);
            return Ok(res);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Got req to delete brand with id {id}");
            serviceService.deleteItem(id);
            return Ok();
        }
    }
}
