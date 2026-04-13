using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Tasks;

namespace TechFlow.Application.Features.Sprints.Commands.CreateSprint
{
    //public class CreateSprintCommandHandler(
    //    IUnitOfWork unitOfWork,
    //    ILogger<CreateSprintCommand> logger,
    //    ProjectAccessService accessService,
    //    IUser currentUser) : IRequestHandler<CreateSprintCommand, Result<SprintSummaryDto>>
    //{
    //    public async Task<Result<SprintSummaryDto>> Handle(CreateSprintCommand command, CancellationToken ct)
    //    {


    //    }
    //}
}
