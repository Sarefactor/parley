using Parley.Workflows.State;

namespace Parley.Core.DataAccess.Models.Variables;

public class VariableContext
{
    public VariableContext(string key,
                           bool isList)
    {
        Key = key;
        IsList = isList;
    }

    public string Key { get; set; } = string.Empty;
    public bool IsList { get; private set; }
    public IterationContext? IterationContext { get; private set; }

    public void SetIterationContext(IterationContext context)
        => IterationContext = context;
}