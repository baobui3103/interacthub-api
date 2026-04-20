using InteractHub.Domain.Entities;

namespace InteractHub.Domain.Core.Specifications
{
    public class FeedPostsPagedSpecification : BaseSpecification<Post>
    {
        public FeedPostsPagedSpecification(int skip, int take) : base(x => true)
        {
            ApplyOrderByDescending(x => x.CreatedOn);
            ApplyPaging(skip, take);
        }
    }
}
