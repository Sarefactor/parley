using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;

namespace Parley.Workflows.Nodes.Nodes.Iterator;

[ParleyNodeValidator]
public class IteratorNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(IteratorNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var isValid = true;

        return isValid;
    }
}
