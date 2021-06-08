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
    public class CarController : ControllerBase
    {
        

        private readonly ILogger<CarController> _logger;
        private readonly ICarService carService;

        public CarController(ILogger<CarController> logger, ICarService brandService)
        {
            _logger = logger;
            this.carService = brandService;
        }

        [HttpGet]
        [Route("all")]
        public List<CarDto> All()
        {
            _logger.LogInformation("Got req to get all cars");
            return carService.getAllItems();
        }

        [HttpGet]
        [Route("byUser")]
        public List<CarDto> GetByUser(int userId)
        {
            _logger.LogInformation("Got req to get all cars by iser id " + userId);
            List<CarDto> cars = carService.GetByUser(userId);
            return cars;
        }

        [HttpPost]
        [Route("add")]
        public CarDto Add(CarDto dto)
        {
            _logger.LogInformation($"Got req to add car {dto}");
            return carService.addItemReturning(dto);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"Got req to get car by id {id}");
            CarDto car = carService.getItem(id);
            return car == null ? NotFound() : Ok(car);
        }

        [HttpPut]
        [Route("edit")]
        public IActionResult Edit(CarDto dto)
        {
            _logger.LogInformation($"Got req to edit car {dto}");
            CarDto res = carService.editItemReturning(dto);
            return Ok(res);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Got req to delete car with id {id}");
            carService.deleteItem(id);
            return Ok();
        }
    }
}
