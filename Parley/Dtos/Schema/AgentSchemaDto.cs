using Parley.Core.DataAccess.Enums;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Dtos.Schema;

[ExportTsClass]
public class AgentSchemaDto
{
    [JsonInclude]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("instructions")]
    public string Instructions { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("workflowSchemas")]
    public List<WorkflowSchemaDto> WorkflowSchemas { get; set; } = new();
}

public class WorkflowSchemaDto
{
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("intent")]
    public string Intent { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("executionNodeId")]
    public Guid ExecutionNodeId { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("nodes")]
    public List<NodeConfigDto> Nodes { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("workflowVariables")]
    public List<WorkflowVariableDto> WorkflowVariables { get; set; } = new();
}

public class NodeConfigDto
{
    [JsonInclude]
    [JsonPropertyName("nodeId")]
    public Guid NodeId { get; set; }

    [JsonInclude]
    [JsonPropertyName("nodeType")]
    public string NodeType { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("primaryTransitionNode")]
    public Guid PrimaryTransitionNode { get; set; }

    [JsonInclude]
    [JsonPropertyName("secondaryTransitionNode")]
    public Guid? SecondaryTransitionNode { get; set; }

    [JsonInclude]
    [JsonPropertyName("nodeOptions")]
    public object NodeOptions { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("nodeVariables")]
    public List<WorkflowVariableDto> NodeVariables { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("transitions")]
    public List<TransitionDto> Transitions { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("validationRules")]
    public List<ValidationRuleDto> ValidationRules { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("position")]
    public NodePositionDto Position { get; set; } = new();
}

public class TransitionDto
{
    [JsonInclude]
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonInclude]
    [JsonPropertyName("targetNodeId")]
    public Guid TargetNodeId { get; set; }

    [JsonInclude]
    [JsonPropertyName("transitionRules")]
    public List<TransitionRuleDto> TransitionRules { get; set; } = new();
}

public class TransitionRuleDto : ValidationRuleDto
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;
}

public class ValidationRuleDto
{
    [JsonInclude]
    [JsonPropertyName("stringComparisonType")]
    public StringComparisonType StringComparisonType { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("matchString")]
    public string? MatchString { get; set; }

    [JsonInclude]
    [JsonPropertyName("regexString")]
    public string? RegexString { get; set; }

    [JsonInclude]
    [JsonPropertyName("numberComparisonType")]
    public NumberComparisonType NumberComparisonType { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("matchInt")]
    public int? MatchInt { get; set; }

    [JsonInclude]
    [JsonPropertyName("boolComparisonType")]
    public BoolComparisonType BoolComparisonType { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("matchBool")]
    public bool? MatchBool { get; set; }

    [JsonInclude]
    [JsonPropertyName("matchDateTime")]
    public DateTime? MatchDateTime { get; set; }
}

public class NodePositionDto
{
    [JsonInclude]
    [JsonPropertyName("x")]
    public double X { get; set; }
    [JsonInclude]
    [JsonPropertyName("y")]
    public double Y { get; set; }
}

public class ParleyVariableDto
{
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    
    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;
    
    [JsonInclude]
    [JsonPropertyName("type")]
    public VariableDataType Type { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("isList")]
    public bool IsList { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("nullable")]
    public bool IsNullable { get; set; }
}

public class WorkflowVariableDto : ParleyVariableDto
{
    [JsonInclude]
    [JsonPropertyName("objectVariables")]
    public List<ParleyVariableDto> ObjectVariables { get; set; } = [];
}