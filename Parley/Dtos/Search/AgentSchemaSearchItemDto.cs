namespace Parley.Dtos.Search;

public class AgentSchemaSearchItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
}