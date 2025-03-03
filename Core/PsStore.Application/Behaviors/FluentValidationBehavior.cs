using FluentValidation;
using MediatR;

namespace PsStore.Application.Behaviors
{
    public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validator;

        public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validator)
        {
            this.validator = validator;
        }
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var failtures = validator
                .Select(v => v.Validate(context))
                .SelectMany(v => v.Errors)
                .GroupBy(v => v.ErrorMessage)
                .Select(v => v.First())
                .Where(f => f is not null)
                .ToList();

            if (failtures.Any())
            {
                throw new ValidationException(failtures);
            }

            return next();
        }
    }
}
