using TypeGen.Core.TypeAnnotations;

namespace Parley.Dtos.Validation;

[ExportTsClass]
public class ParleyValidationContextDto
{
    public List<string> AgentErrorMessages { get; set; } = [];
    public List<ParleyWorkflowValidationErrorDto> WorkflowErrors { get; set; } = [];
}