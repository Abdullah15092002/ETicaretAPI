using ETicaretAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Services.Authentication
{
   public interface IInternalAuthentication
    {
        Task<Token> LoginAsync(string usernameOrEmail, string password,int accesTokenLifeTime);
        Task<Token> RefreshTokenLoginAsync(string refreshToken);
    }
}
