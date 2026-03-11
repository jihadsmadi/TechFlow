namespace TechFlow.Application.Features.Boards.DTOs;

public sealed record BoardDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    IReadOnlyList<ListDto> Lists,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ListDto(
    Guid Id,
    Guid BoardId,
    string Name,
    string? Color,
    int DisplayOrder,
    bool IsDefault,
    bool IsCompletedList,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ListSummaryDto(
    Guid Id,
    string Name,
    string? Color,
    int DisplayOrder,
    bool IsDefault,
    bool IsCompletedList);
