namespace InteractHub.Application.Models.Requests
{
    public class UpdatePostReq
    {
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
