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
    public class UserServiceOld : IUserService
    {
        private readonly ILogger<UserServiceOld> _logger;
        private readonly UnitOfWork repos;

        public UserServiceOld(ILogger<UserServiceOld> logger, UnitOfWork repos)
        {
            this._logger = logger;
            this.repos = repos;
        }

        public UserDto addItemReturning(UserDto userDto)
        {
            _logger.LogInformation($"Adding user to db {userDto}");
            User result = repos.Users.Save(new User { Name = userDto.Name });
            return new UserDto { Id = result.Id, Name = result.Name };
        }

        public UserDto getItem(int id)
        {
            User user = repos.Users.FindByIdWithoutIncludes(id);
            return user == null ? null : new UserDto { Id = user.Id, Name = user.Name };
        }

        public UserDto editItemReturning(UserDto userDto)
        {
            User user = new User { Id = userDto.Id, Name = userDto.Name };
            if (repos.Users.Contains(user))
            {
                User res = repos.Users.Update(user);
                return new UserDto { Id = res.Id, Name = res.Name };
            }

            return null;
        }

        public UserDto deleteItemReturning(int id)
        {
            if (repos.Users.FindByIdWithoutIncludes(id) != null)
            {
                User res = repos.Users.Delete(id);
                return new UserDto { Id = res.Id, Name = res.Name };
            }

            return null;
        }

        public List<UserDto> getAllItems()
        {
            return repos.Users.FindAll().ConvertAll<UserDto>(
                user =>
                {
                    return new UserDto { Id = user.Id, Name = user.Name };
                }
                );
        }

        public void addItem(UserDto itemDto)
        {
            throw new NotImplementedException();
        }

        public void editItem(UserDto itemDto)
        {
            throw new NotImplementedException();
        }

        public void deleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}
