using Parley.Workflows.State;

namespace Parley.Core.DataAccess.Models.Variables;

public class VariableIterationContext
{
    public VariableIterationContext(string primaryKey,
                                   bool primaryIsList,
                                   string? secondaryKey = null,
                                   bool? secondaryIsList = null)
    {
        PrimaryContext = new VariableContext(primaryKey, primaryIsList);

        if (secondaryKey != null && secondaryIsList != null)
            SecondaryContext = new VariableContext(secondaryKey, (bool)secondaryIsList);
    }

    public VariableContext PrimaryContext { get; private set; }
    public VariableContext? SecondaryContext { get; private set; }

    public void SetIterationContext(IterationContext context,
                                    bool isPrimary)
        => (isPrimary ? PrimaryContext : SecondaryContext)?.SetIterationContext(context);

    public bool HasList()
        => PrimaryContext.IsList || SecondaryContext is { IsList: true };
}