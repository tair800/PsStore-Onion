using MediatR;

namespace PsStore.Application.Features.Category.Commands.RestoreCategory
{
    public class RestoreCategoryCommandRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
