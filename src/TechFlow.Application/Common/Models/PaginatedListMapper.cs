
namespace TechFlow.Application.Common.Models;

public static class PaginatedListExtensions
{
    public static PaginatedList<TDestination> ToPaginatedList<TSource, TDestination>(
        this PaginatedList<TSource> source,
        Func<TSource, TDestination> mapper)
    {
        var mappedItems = source.Items.Select(mapper).ToList();

        return new PaginatedList<TDestination>(
            mappedItems,
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
    }
}