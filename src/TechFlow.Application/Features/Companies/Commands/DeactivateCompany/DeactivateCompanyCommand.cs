using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Commands.DeactivateCompany;

public sealed record DeactivateCompanyCommand(Guid Id) : IRequest<Result<Updated>>;