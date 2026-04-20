using InteractHub.Application.Models.DTOs;

namespace InteractHub.Application.Models.Responses
{
    public class UpsertPostRes
    {
        public string Message { get; set; } = string.Empty;
        public PostDTO Data { get; set; } = new();
    }
}
