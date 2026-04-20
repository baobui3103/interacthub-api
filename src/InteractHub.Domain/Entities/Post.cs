using InteractHub.Domain.Core.Models;

namespace InteractHub.Domain.Entities
{
    public class Post : BaseEntity, IAuditableEntity, ISoftDeleteEntity
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }
}
