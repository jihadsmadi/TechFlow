using FluentValidation;
using MediatR;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Common.Results.Abstractions;

namespace TechFlow.Application.Common.Behaviours;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (validator is null)
            return await next(ct);

        var validationResult = await validator.ValidateAsync(request, ct);

        if (validationResult.IsValid)
            return await next(ct);

        var errors = validationResult.Errors
            .ConvertAll(failure => Error.Validation(
                code: failure.PropertyName,
                description: failure.ErrorMessage));

        return (TResponse)(dynamic)(List<Error>)errors;
    }
}