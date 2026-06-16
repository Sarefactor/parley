using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Schemas;

public class WorkflowSchema
{
    public WorkflowSchema(Guid executionNodeId,
                          string name,
                          string intent,
                          string description)
    {
        Name = name;
        Intent = intent;
        Description = description;
        ExecutionNodeId = executionNodeId;
    }


    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("intent")]
    public string Intent { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("executionNodeId")]
    public Guid ExecutionNodeId { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("nodes")]
    public Dictionary<Guid, NodeConfig> Nodes { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("workflowVariables")]
    public List<WorkflowVariable> WorkflowVariables { get; private set; } = new();

    public List<WorkflowVariable> GetAllWorkflowVariables()
        => Nodes.SelectMany(x => x.Value.NodeVariables)
                .Concat(WorkflowVariables)
                .ToList();

    public void SetWorkflowVariables(List<WorkflowVariable> workflowVariables)
        => WorkflowVariables = workflowVariables;

    public void SetWorkflowNodes(Dictionary<Guid, NodeConfig> nodes)
        => Nodes = nodes;
}