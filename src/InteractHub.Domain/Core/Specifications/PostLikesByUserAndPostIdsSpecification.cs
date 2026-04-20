using InteractHub.Domain.Entities;

namespace InteractHub.Domain.Core.Specifications
{
    public class PostLikesByUserAndPostIdsSpecification : BaseSpecification<PostLike>
    {
        public PostLikesByUserAndPostIdsSpecification(Guid userId, IList<Guid> postIds)
            : base(x => x.UserId == userId && postIds.Contains(x.PostId))
        {
        }
    }
}
