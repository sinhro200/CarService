using Business.DTO;
using Business.Interfaces;
using Core;
using Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UnitOfWork repos;

        public UserService(ILogger<UserService> logger, UnitOfWork repos)
        {
            this._logger = logger;
            this.repos = repos;
        }

        public UserDto addUser(UserDto userDto)
        {
            _logger.LogInformation($"Adding user to db {userDto.Name}");
            User result = repos.Users.Save(new User { Name = userDto.Name });
            return new UserDto { Id = result.Id, Name = result.Name };
        }

        public List<UserDto> getAllUsers()
        {
            return repos.Users.FindAll().ConvertAll<UserDto>(
                user =>
                {
                    return new UserDto{ Id = user.Id, Name = user.Name };
                }
                );
        }

        public UserDto getUser(int id)
        {
            User user = repos.Users.FindById(id);
            return user == null ? null : new UserDto { Id = user.Id, Name = user.Name };
        }
    }
}
