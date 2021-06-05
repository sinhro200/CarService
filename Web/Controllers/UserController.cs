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
            _logger.LogInformation("Get all users");
            return userService.getAllUsers();
        }

        [HttpPost]
        [Route("add")]
        public UserDto Add(UserDto user)
        {
            _logger.LogInformation($"add user {user}");
            return userService.addUser(user);
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"get user by id {id}");
            UserDto user = userService.getUser(id);
            return user == null ? NotFound() : Ok(user);
        }
    }
}
