using Microsoft.AspNetCore.Identity;
using InteractHub.Domain.Core.Models;
using InteractHub.Domain.Enums;

namespace InteractHub.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity, ISoftDeleteEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserStatus Status { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
