using InteractHub.Application.Models.Requests;
using InteractHub.Application.Models.Responses;

namespace InteractHub.Application.Interfaces
{
    public interface IPostService
    {
        Task<UpsertPostRes> CreatePost(Guid currentUserId, CreatePostReq request);
        Task<UpsertPostRes?> UpdatePost(Guid currentUserId, Guid postId, UpdatePostReq request);
        Task<bool> DeletePost(Guid currentUserId, Guid postId);
        Task<ToggleLikePostRes?> ToggleLike(Guid currentUserId, Guid postId);
        Task<GetPostsRes> GetFeed(Guid currentUserId, int pageNumber, int pageSize);
        Task<GetPostsRes> GetMyPosts(Guid currentUserId, int pageNumber, int pageSize);
        Task<GetPostByIdRes?> GetById(Guid currentUserId, Guid postId);
    }
}
