
namespace TechFlow.Application.Features.Companies.Dtos
{
    public sealed class CompanySummaryDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string ContactEmail { get; init; } = string.Empty;
        public string Industry { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public int FeatureFlagsCount { get; init; } 
    }
}
