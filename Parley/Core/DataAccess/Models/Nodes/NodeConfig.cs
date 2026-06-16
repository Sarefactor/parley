using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Nodes;

public class NodeConfig
{
    public NodeConfig(Guid nodeId,
                      string nodeType,
                      Guid primaryTransitionNode,
                      Guid? secondaryTransitionNode)
    {
        NodeId = nodeId;
        NodeType = nodeType;
        PrimaryTransitionNode = primaryTransitionNode;
        SecondaryTransitionNode = secondaryTransitionNode;
    }

    [JsonInclude]
    [JsonPropertyName("nodeId")]
    public Guid NodeId { get; private set; }

    [JsonInclude]
    [JsonPropertyName("nodeType")]
    public string NodeType { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("primaryTransitionNode")]
    public Guid PrimaryTransitionNode { get; private set; }

    [JsonInclude]
    [JsonPropertyName("secondaryTransitionNode")]
    public Guid? SecondaryTransitionNode { get; private set; }

    [JsonInclude]
    [JsonPropertyName("options")]
    public JsonElement Options { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("nodeVariables")]
    public List<WorkflowVariable> NodeVariables { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("transitions")]
    public List<Transition> Transitions { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("validationRules")]
    public List<ValidationRule> ValidationRules { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("nodePosition")]
    public NodePosition NodePosition { get; private set; } = new NodePosition(0, 0);

    public void SetNodeVariables(List<WorkflowVariable> nodeVariables)
        => NodeVariables = nodeVariables;

    public void SetTransitions(List<Transition> transitions)
        => Transitions = transitions;

    public void SetValidationRules(List<ValidationRule> validationRules)
        => ValidationRules = validationRules;

    public void SetNodePosition(NodePosition nodePosition)
        => NodePosition = nodePosition;

    public void SetOptions(object options)
        => Options = (JsonElement)options;
}