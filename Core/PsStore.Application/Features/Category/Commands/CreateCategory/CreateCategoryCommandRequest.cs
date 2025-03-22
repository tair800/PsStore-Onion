using MediatR;

namespace PsStore.Application.Features.Category.Commands
{
    public class CreateCategoryCommandRequest : IRequest<Result<Unit>>
    {
        public string Name { get; set; }
    }
}
