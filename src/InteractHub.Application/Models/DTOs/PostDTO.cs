namespace InteractHub.Application.Models.DTOs
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
