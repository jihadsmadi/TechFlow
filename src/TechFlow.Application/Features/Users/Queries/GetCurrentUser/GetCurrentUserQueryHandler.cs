using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Users.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Users;

namespace TechFlow.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(
        GetCurrentUserQuery query,
        CancellationToken   ct)
    {
        var user = await unitOfWork.Users.GetByIdAsync(query.UserId, ct);
        if (user is null)
            return UserErrors.NotFound;

        var companyRoles = await unitOfWork.Users.GetCompanyRolesAsync(user.Id, ct);

        return new UserProfileDto(
            Id:        user.Id,
            Email:     user.Email,
            FirstName: user.FirstName,
            LastName:  user.LastName,
            FullName:  user.FullName,
            AvatarUrl: user.AvatarUrl,
            IsActive:  user.IsActive,
            CompanyId: user.CompanyId,
            Role:      companyRoles.RoleNames.FirstOrDefault() ?? string.Empty,
            Preferences: new UserPreferencesDto(
                Theme:                user.Preferences.Theme,
                BoardView:            user.Preferences.BoardView,
                Language:             user.Preferences.Language,
                NotifyOnTaskAssigned: user.Preferences.NotifyOnTaskAssigned,
                NotifyOnCommentAdded: user.Preferences.NotifyOnCommentAdded,
                NotifyOnDueDateNear:  user.Preferences.NotifyOnDueDateNear,
                NotifyOnTaskMoved:    user.Preferences.NotifyOnTaskMoved,
                NotifyOnMentioned:    user.Preferences.NotifyOnMentioned,
                DueDateReminderDays:  user.Preferences.DueDateReminderDays));
    }
}
