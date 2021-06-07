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
    public class ModelController : ControllerBase
    {
        

        private readonly ILogger<ModelController> _logger;
        private readonly IModelService modelService;

        public ModelController(ILogger<ModelController> logger, IModelService modelService)
        {
            _logger = logger;
            this.modelService = modelService;
        }

        [HttpGet]
        [Route("all")]
        public List<ModelDto> All()
        {
            _logger.LogInformation("Got req to get all models");
            return modelService.getAllItems();
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(ModelDto dto)
        {
            _logger.LogInformation($"Got req to add model {dto}");
            ModelDto res = modelService.addItemReturning(dto);
            return Ok(res);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"Got req to get model by id {id}");
            ModelDto res = modelService.getItem(id);
            return res == null ? NotFound() : Ok(res);
        }

        [HttpPut]
        [Route("edit")]
        public IActionResult Edit(ModelDto dto)
        {
            _logger.LogInformation($"Got req to edit model {dto}");
            ModelDto res = modelService.editItemReturning(dto);
            return Ok(res);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Got req to delete model with id {id}");
            modelService.deleteItem(id);
            return Ok();
        }
    }
}
