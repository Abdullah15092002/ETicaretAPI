using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Services;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class AuthService : IAuthService
    {
        readonly IUserService _userService;
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly ITokenHandler _tokenHandler;
        public AuthService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, ITokenHandler tokenHandler, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
            _userService = userService;

        }
        public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accesTokenLifeTime)
        {

            AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(usernameOrEmail);
            }
            if (user == null)
            {
                throw new NotFoundUserException() { };
            }


            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {

                Token token = _tokenHandler.CreateAccesToken(accesTokenLifeTime,user);
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 900);
                return token;
            }
            throw new AuthenticationErrorException();
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
          AppUser? user=await  _userManager.Users.FirstOrDefaultAsync(u=>u.RefreshToken==refreshToken);
            if (user != null && user?.RefreshTokenEndDate>DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccesToken(900,user);
                await _userService.UpdateRefreshToken(token.RefreshToken,user, token.Expiration, 300);
                return token;
            }else
            throw new NotFoundUserException();
        }
    }
}
