using InteractHub.Application.Models.DTOs;

namespace InteractHub.Application.Models.Responses
{
    public class GetAllActiveUsersRes
    {
        public IList<UserDTO> Data { get; set; }
    }
}
