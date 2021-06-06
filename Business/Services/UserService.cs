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
    public class UserService : DefaultService<UserDto,User,UserService>, IUserService
    {

        public UserService(ILogger<UserService> logger, UnitOfWork repos) 
            : base(repos.Users,logger,
                  dto =>
                  {
                      return new User { Id = dto.Id, Name = dto.Name };
                  },
                  model =>
                  {
                      return new UserDto { Id = model.Id, Name = model.Name };
                  }
                  )
        {}
    }
}
