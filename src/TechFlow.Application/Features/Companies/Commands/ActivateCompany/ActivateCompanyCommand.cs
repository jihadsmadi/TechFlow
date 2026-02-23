using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Commands.ActivateCompany;

public sealed record ActivateCompanyCommand(Guid Id) : IRequest<Result<Updated>>;