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
    public class BrandController : ControllerBase
    {
        

        private readonly ILogger<BrandController> _logger;
        private readonly IBrandService brandService;

        public BrandController(ILogger<BrandController> logger, IBrandService brandService)
        {
            _logger = logger;
            this.brandService = brandService;
        }

        [HttpGet]
        [Route("all")]
        public List<BrandDto> All()
        {
            _logger.LogInformation("Got req to get all brands");
            return brandService.getAllItems();
        }

        [HttpPost]
        [Route("add")]
        public BrandDto Add(BrandDto dto)
        {
            _logger.LogInformation($"Got req to add brand {dto}");
            return brandService.addItemReturning(dto);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"Got req to get brand by id {id}");
            BrandDto brand = brandService.getItem(id);
            return brand == null ? NotFound() : Ok(brand);
        }

        [HttpPut]
        [Route("edit")]
        public IActionResult Edit(BrandDto dto)
        {
            _logger.LogInformation($"Got req to edit brand {dto}");
            BrandDto res = brandService.editItemReturning(dto);
            return Ok(res);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Got req to delete brand with id {id}");
            brandService.deleteItem(id);
            return Ok();
        }
    }
}
