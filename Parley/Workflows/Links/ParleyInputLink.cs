namespace Parley.Workflows.Links;

public class ParleyInputLink
{
    public ParleyInputType Type { get; set; } = ParleyInputType.Plain;
    public string Message { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = [];
}