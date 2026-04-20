namespace InteractHub.Application.Models.Responses
{
    public class ToggleLikePostRes
    {
        public Guid PostId { get; set; }
        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
    }
}
