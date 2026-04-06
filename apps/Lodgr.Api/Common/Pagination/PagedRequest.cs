using System.ComponentModel.DataAnnotations;

namespace Lodgr.Api.Common.Pagination;

public class PagedRequest
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    public int Skip => (PageNumber - 1) * PageSize;
}
