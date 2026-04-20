using InteractHub.Domain.Entities;

namespace InteractHub.Domain.Core.Specifications
{
    public class MyPostsPagedSpecification : BaseSpecification<Post>
    {
        public MyPostsPagedSpecification(Guid userId, int skip, int take) : base(x => x.UserId == userId)
        {
            ApplyOrderByDescending(x => x.CreatedOn);
            ApplyPaging(skip, take);
        }
    }
}
