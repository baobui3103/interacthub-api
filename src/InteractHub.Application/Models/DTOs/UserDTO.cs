using InteractHub.Domain.Entities;

namespace InteractHub.Application.Models.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusText { get; set; } = string.Empty;

        public UserDTO(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailId = user.Email ?? string.Empty;
            Status = (int)user.Status;
            StatusText = user.Status.ToString();
        }
    }
}
