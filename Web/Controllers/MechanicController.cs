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
    public class MechanicController : ControllerBase
    {
        

        private readonly ILogger<MechanicController> _logger;
        private readonly IMechanicService mechanicService;

        public MechanicController(ILogger<MechanicController> logger, IMechanicService mechanicService)
        {
            _logger = logger;
            this.mechanicService = mechanicService;
        }

        [HttpGet]
        [Route("all")]
        public List<MechanicDto> AllMechanics()
        {
            _logger.LogInformation("Got req to get all mechanics");
            return mechanicService.getAllItems();
        }

        [HttpPost]
        [Route("add")]
        public MechanicDto Add(MechanicDto mechanic)
        {
            _logger.LogInformation($"Got req to add mechanic {mechanic}");
            return mechanicService.addItemReturning(mechanic);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"Got req to get mechanic by id {id}");
            MechanicDto user = mechanicService.getItem(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut]
        [Route("edit")]
        public MechanicDto Edit(MechanicDto mechanic)
        {
            _logger.LogInformation($"Got req to edit mechanic {mechanic}");
            return mechanicService.editItemReturning(mechanic);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Got req to delete mechanic with id {id}");
            mechanicService.deleteItem(id);
            return Ok();
        }
    }
}
