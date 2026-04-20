using InteractHub.Domain.Entities;

namespace InteractHub.Domain.Core.Specifications
{
    public class AllPostsCountSpecification : BaseSpecification<Post>
    {
        public AllPostsCountSpecification() : base(x => true)
        {
        }
    }
}
