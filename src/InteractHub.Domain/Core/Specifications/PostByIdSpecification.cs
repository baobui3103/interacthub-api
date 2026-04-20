using InteractHub.Domain.Entities;

namespace InteractHub.Domain.Core.Specifications
{
    public class PostByIdSpecification : BaseSpecification<Post>
    {
        public PostByIdSpecification(Guid postId) : base(x => x.Id == postId)
        {
        }
    }
}
