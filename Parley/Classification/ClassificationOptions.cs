namespace Parley.Classification;

public class ClassificationOptions
{
    public string Text { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsWorkflowClassification = false;
    public List<string> ClassificationVariables = [];
}
