using MediatR;

namespace PsStore.Application.Features.Category.Commands.DeleteCategory
{
    public class DeleteCategoryCommandRequest : IRequest<Result<Unit>>
    {
        public int Id { get; set; }
    }
}
