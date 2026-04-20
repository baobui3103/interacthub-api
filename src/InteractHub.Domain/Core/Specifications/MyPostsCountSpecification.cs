using InteractHub.Domain.Entities;

namespace InteractHub.Domain.Core.Specifications
{
    public class MyPostsCountSpecification : BaseSpecification<Post>
    {
        public MyPostsCountSpecification(Guid userId) : base(x => x.UserId == userId)
        {
        }
    }
}
