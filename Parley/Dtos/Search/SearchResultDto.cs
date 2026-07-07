using TypeGen.Core.TypeAnnotations;

namespace Parley.Dtos.Search;

[ExportTsClass]
public class SearchResultDto<T>
{
    public int TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<T> Results { get; set; } = [];
}