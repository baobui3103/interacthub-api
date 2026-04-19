using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Domain.Enums;
using InteractHub.Application.Models.Responses;
using InteractHub.Application.Models.DTOs;
using InteractHub.Application.Interfaces;
using InteractHub.Application.Core.Services;

namespace InteractHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _loggerService;

        public UserService(UserManager<ApplicationUser> userManager, ILoggerService loggerService)
        {
            _userManager = userManager;
            _loggerService = loggerService;
        }

        public async Task<GetAllActiveUsersRes> GetAllActiveUsers()
        {
            var users = await _userManager.Users
                .Where(x => x.Status == UserStatus.Active)
                .ToListAsync();

            _loggerService.LogInfo("Listed active users");

            return new GetAllActiveUsersRes()
            {
                Data = users.Select(x => new UserDTO(x)).ToList()
            };
        }
    }
}
