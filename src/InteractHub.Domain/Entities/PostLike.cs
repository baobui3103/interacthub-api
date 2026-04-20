using InteractHub.Domain.Core.Models;

namespace InteractHub.Domain.Entities
{
    public class PostLike : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public Post Post { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
