using InteractHub.Application.Models.Responses;

namespace InteractHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<GetAllActiveUsersRes> GetAllActiveUsers();
    }
}
