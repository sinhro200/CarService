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
    public class UserController : ControllerBase
    {
        

        private readonly ILogger<UserController> _logger;
        private readonly IUserService userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            this.userService = userService;
        }

        [HttpGet]
        [Route("all")]
        public List<UserDto> AllUsers()
        {
            _logger.LogInformation("Got req to get all users");
            return userService.getAllItems();
        }

        [HttpPost]
        [Route("add")]
        public UserDto Add(UserDto user)
        {
            _logger.LogInformation($"Got req to add user {user}");
            return userService.addItem(user);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"Got req to get user by id {id}");
            UserDto user = userService.getItem(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut]
        [Route("edit")]
        public UserDto Edit(UserDto user)
        {
            _logger.LogInformation($"Got req to edit user {user}");
            return userService.editItem(user);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public UserDto Delete(int id)
        {
            _logger.LogInformation($"Got req to delete user with id {id}");
            return userService.deleteItem(id);
        }
    }
}
