using InteractHub.Application.Models.DTOs;

namespace InteractHub.Application.Models.Responses
{
    public class GetPostsRes
    {
        public IList<PostDTO> Data { get; set; } = new List<PostDTO>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
