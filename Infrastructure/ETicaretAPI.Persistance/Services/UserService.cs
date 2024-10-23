using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Services;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<AppUser> _userManager;
        public UserService(UserManager<AppUser> userManager)
        {
            _userManager=userManager;
        }
        public async Task<DTOCreateUserResponse> CreateAsync(DTOCreateUserRequest model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                NameSurname = model.NameSurname,
                UserName = model.UserName,
                Email = model.Email,
            }, model.Password);

            DTOCreateUserResponse response = new() { Succeeded = result.Succeeded };
            if (result.Succeeded)
            {
                response.Message = "Kullanıcı Başarıyla Oluşturulmuştur";
            }
            else
            {
                response.Message = "Kullanıcı Oluşturulurken Hata Oluştu";

            }
            return response;
        }

        public async Task UpdateRefreshToken(string refreshToken, AppUser user,DateTime accesTokenDate,int addOnAccesToken)
        {
          
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accesTokenDate.AddSeconds(addOnAccesToken);
                await _userManager.UpdateAsync(user);
            }else
            throw new NotFoundUserException();
        }
    }
}
