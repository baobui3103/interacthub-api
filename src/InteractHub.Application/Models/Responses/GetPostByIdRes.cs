using InteractHub.Application.Models.DTOs;

namespace InteractHub.Application.Models.Responses
{
    public class GetPostByIdRes
    {
        public PostDTO Data { get; set; } = new();
    }
}
