using MediatR;
using Microsoft.AspNetCore.Identity;
using Identity=ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Abstractions.Hubs;

namespace ETicaretAPI.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        readonly IUserService _userService;
        
        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
            
        }
        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            DTOCreateUserResponse response =await _userService.CreateAsync(new()
            {
                Email = request.Email,
                NameSurname = request.NameSurname,
                Password = request.Password,
                UserName = request.UserName,
                PasswordConfirm = request.PasswordConfirm,
            });
            
            return new()
            {
                Message = response.Message,
                Succeeded = response.Succeeded,
            };
        }
    }
}
