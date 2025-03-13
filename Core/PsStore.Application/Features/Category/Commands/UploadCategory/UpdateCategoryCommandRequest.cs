using MediatR;

namespace PsStore.Application.Features.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommandRequest : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
