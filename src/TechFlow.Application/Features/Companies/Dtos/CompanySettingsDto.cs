namespace TechFlow.Application.Features.Companies.Dtos
{
    public class CompanySettingsDto
    {
        public string PrimaryColor { get; init; } = string.Empty;
        public string? LogoUrl { get; init; }
        public string? CompanyWebsite { get; init; }
        public string DefaultTimezone { get; init; } = string.Empty;
        public string DateFormat { get; init; } = string.Empty;
        public string Language { get; init; } = string.Empty;
        public bool AllowGuestAccess { get; init; }
        public bool RequireTaskDueDate { get; init; }
        public bool AllowMembersInvite { get; init; }

    }
}