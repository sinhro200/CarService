using Business.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUserService
    {
        public UserDto addUser(UserDto userDto);

        public UserDto getUser(int id);

        public List<UserDto> getAllUsers(); 
    }
}
