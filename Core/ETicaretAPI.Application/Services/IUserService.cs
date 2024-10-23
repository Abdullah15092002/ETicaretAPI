using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Services
{
    public  interface IUserService
    {
        Task<DTOCreateUserResponse> CreateAsync(DTOCreateUserRequest model);
        Task UpdateRefreshToken(string refreshToken,AppUser user,DateTime accesTokenDate, int addOnAccesToken);
        
    }
}
