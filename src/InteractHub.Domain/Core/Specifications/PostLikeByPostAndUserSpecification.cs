using InteractHub.Domain.Entities;

namespace InteractHub.Domain.Core.Specifications
{
    public class PostLikeByPostAndUserSpecification : BaseSpecification<PostLike>
    {
        public PostLikeByPostAndUserSpecification(Guid postId, Guid userId)
            : base(x => x.PostId == postId && x.UserId == userId)
        {
        }
    }
}
