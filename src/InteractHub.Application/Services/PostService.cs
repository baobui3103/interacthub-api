using InteractHub.Application.Core.Services;
using InteractHub.Application.Interfaces;
using InteractHub.Application.Models.DTOs;
using InteractHub.Application.Models.Requests;
using InteractHub.Application.Models.Responses;
using InteractHub.Domain.Core.Repositories;
using InteractHub.Domain.Core.Specifications;
using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace InteractHub.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _loggerService;

        public PostService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ILoggerService loggerService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _loggerService = loggerService;
        }

        public async Task<UpsertPostRes> CreatePost(Guid currentUserId, CreatePostReq request)
        {
            var content = request.Content.Trim();
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Nội dung bài đăng không được để trống.");
            }

            var postRepository = _unitOfWork.Repository<Post>();
            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                Content = content,
                ImageUrl = NormalizeImageUrl(request.ImageUrl),
                CreatedBy = currentUserId
            };

            await postRepository.AddAsync(post);
            await _unitOfWork.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(currentUserId.ToString())
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

            _loggerService.LogInfo($"User {currentUserId} created post {post.Id}");

            return new UpsertPostRes
            {
                Message = "Tạo bài đăng thành công.",
                Data = ToPostDto(post, user.FirstName, user.LastName, false)
            };
        }

        public async Task<UpsertPostRes?> UpdatePost(Guid currentUserId, Guid postId, UpdatePostReq request)
        {
            var postRepository = _unitOfWork.Repository<Post>();
            var post = await postRepository.FirstOrDefaultAsync(new PostByIdSpecification(postId));
            if (post == null)
            {
                return null;
            }

            if (post.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền sửa bài đăng này.");
            }

            var content = request.Content.Trim();
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Nội dung bài đăng không được để trống.");
            }

            post.Content = content;
            post.ImageUrl = NormalizeImageUrl(request.ImageUrl);
            post.LastModifiedBy = currentUserId;

            postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(currentUserId.ToString())
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

            var likeRepository = _unitOfWork.Repository<PostLike>();
            var isLiked = await likeRepository.FirstOrDefaultAsync(
                new PostLikeByPostAndUserSpecification(postId, currentUserId)) != null;

            _loggerService.LogInfo($"User {currentUserId} updated post {post.Id}");

            return new UpsertPostRes
            {
                Message = "Cập nhật bài đăng thành công.",
                Data = ToPostDto(post, user.FirstName, user.LastName, isLiked)
            };
        }

        public async Task<bool> DeletePost(Guid currentUserId, Guid postId)
        {
            var postRepository = _unitOfWork.Repository<Post>();
            var post = await postRepository.FirstOrDefaultAsync(new PostByIdSpecification(postId));
            if (post == null)
            {
                return false;
            }

            if (post.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bài đăng này.");
            }

            post.IsDeleted = true;
            post.LastModifiedBy = currentUserId;
            postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync();

            _loggerService.LogInfo($"User {currentUserId} deleted post {post.Id}");
            return true;
        }

        public async Task<ToggleLikePostRes?> ToggleLike(Guid currentUserId, Guid postId)
        {
            var postRepository = _unitOfWork.Repository<Post>();
            var likeRepository = _unitOfWork.Repository<PostLike>();

            var post = await postRepository.FirstOrDefaultAsync(new PostByIdSpecification(postId));
            if (post == null)
            {
                return null;
            }

            var like = await likeRepository.FirstOrDefaultAsync(
                new PostLikeByPostAndUserSpecification(postId, currentUserId));

            var isLiked = false;
            if (like == null)
            {
                await likeRepository.AddAsync(new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = currentUserId,
                    CreatedOn = DateTimeOffset.UtcNow
                });
                post.LikeCount++;
                isLiked = true;
            }
            else
            {
                likeRepository.Delete(like);
                post.LikeCount = Math.Max(post.LikeCount - 1, 0);
            }

            postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync();

            _loggerService.LogInfo($"User {currentUserId} toggled like for post {postId}. IsLiked: {isLiked}");

            return new ToggleLikePostRes
            {
                PostId = postId,
                IsLiked = isLiked,
                LikeCount = post.LikeCount
            };
        }

        public async Task<GetPostsRes> GetFeed(Guid currentUserId, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);
            var skip = (pageNumber - 1) * pageSize;

            var postRepository = _unitOfWork.Repository<Post>();
            var posts = await postRepository.ListAsync(new FeedPostsPagedSpecification(skip, pageSize));
            var totalCount = await postRepository.CountAsync(new AllPostsCountSpecification());

            return await BuildPostListResponse(posts, totalCount, currentUserId, pageNumber, pageSize);
        }

        public async Task<GetPostsRes> GetMyPosts(Guid currentUserId, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);
            var skip = (pageNumber - 1) * pageSize;

            var postRepository = _unitOfWork.Repository<Post>();
            var posts = await postRepository.ListAsync(new MyPostsPagedSpecification(currentUserId, skip, pageSize));
            var totalCount = await postRepository.CountAsync(new MyPostsCountSpecification(currentUserId));

            return await BuildPostListResponse(posts, totalCount, currentUserId, pageNumber, pageSize);
        }

        public async Task<GetPostByIdRes?> GetById(Guid currentUserId, Guid postId)
        {
            var postRepository = _unitOfWork.Repository<Post>();
            var likeRepository = _unitOfWork.Repository<PostLike>();
            var post = await postRepository.FirstOrDefaultAsync(new PostByIdSpecification(postId));
            if (post == null)
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(post.UserId.ToString())
                ?? throw new InvalidOperationException("Không tìm thấy tác giả bài đăng.");

            var isLiked = await likeRepository.FirstOrDefaultAsync(
                new PostLikeByPostAndUserSpecification(postId, currentUserId)) != null;

            return new GetPostByIdRes
            {
                Data = ToPostDto(post, user.FirstName, user.LastName, isLiked)
            };
        }

        private async Task<GetPostsRes> BuildPostListResponse(
            IList<Post> posts,
            int totalCount,
            Guid currentUserId,
            int pageNumber,
            int pageSize)
        {
            var likeRepository = _unitOfWork.Repository<PostLike>();
            var postIds = posts.Select(x => x.Id).ToList();
            var likes = postIds.Count == 0
                ? new List<PostLike>()
                : (await likeRepository.ListAsync(new PostLikesByUserAndPostIdsSpecification(currentUserId, postIds))).ToList();
            var likedPostIds = likes.Select(x => x.PostId).ToHashSet();

            var authorIds = posts.Select(x => x.UserId).Distinct().ToList();
            var authorMap = new Dictionary<Guid, string>();
            foreach (var authorId in authorIds)
            {
                var user = await _userManager.FindByIdAsync(authorId.ToString());
                if (user != null)
                {
                    authorMap[authorId] = $"{user.FirstName} {user.LastName}".Trim();
                }
            }

            var data = posts.Select(x =>
            {
                authorMap.TryGetValue(x.UserId, out var authorName);
                return new PostDTO
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    AuthorName = authorName ?? string.Empty,
                    Content = x.Content,
                    ImageUrl = x.ImageUrl,
                    LikeCount = x.LikeCount,
                    CommentCount = x.CommentCount,
                    IsLikedByCurrentUser = likedPostIds.Contains(x.Id),
                    CreatedOn = x.CreatedOn
                };
            }).ToList();

            return new GetPostsRes
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        private static string? NormalizeImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return null;
            }

            return imageUrl.Trim();
        }

        private static PostDTO ToPostDto(Post post, string firstName, string lastName, bool isLikedByCurrentUser)
        {
            return new PostDTO
            {
                Id = post.Id,
                UserId = post.UserId,
                AuthorName = $"{firstName} {lastName}".Trim(),
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                IsLikedByCurrentUser = isLikedByCurrentUser,
                CreatedOn = post.CreatedOn
            };
        }
    }
}
