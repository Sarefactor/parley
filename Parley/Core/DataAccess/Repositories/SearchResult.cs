namespace Parley.Core.DataAccess.Repositories;

public class SearchResult<T>
{
    public int TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<T> Results { get; set; } = [];
}