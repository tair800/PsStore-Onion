using MediatR;

namespace PsStore.Application.Features.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommandRequest : IRequest<Result<Unit>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> GameIds { get; set; } = new();
    }
}
