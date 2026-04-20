using InteractHub.Application.Interfaces;
using InteractHub.Application.Models.Requests;
using InteractHub.Application.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InteractHub.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<GetPostsRes>> GetFeed([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _postService.GetFeed(currentUserId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<GetPostsRes>> GetMyPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _postService.GetMyPosts(currentUserId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{postId:guid}")]
        public async Task<ActionResult<GetPostByIdRes>> GetById(Guid postId)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _postService.GetById(currentUserId, postId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<UpsertPostRes>> Create([FromBody] CreatePostReq request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _postService.CreatePost(currentUserId, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{postId:guid}")]
        public async Task<ActionResult<UpsertPostRes>> Update(Guid postId, [FromBody] UpdatePostReq request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _postService.UpdatePost(currentUserId, postId, request);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> Delete(Guid postId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var deleted = await _postService.DeletePost(currentUserId, postId);
                if (!deleted)
                {
                    return NotFound();
                }

                return Ok(new { message = "Xóa bài đăng thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpPost("{postId:guid}")]
        public async Task<ActionResult<ToggleLikePostRes>> ToggleLike(Guid postId)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _postService.ToggleLike(currentUserId, postId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
            }

            return userId;
        }
    }
}
