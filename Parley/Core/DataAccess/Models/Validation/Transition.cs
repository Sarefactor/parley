using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Validation;

public class Transition
{
    public Transition(int priority,
                      Guid targetNodeId)
    {
        Priority = priority;
        TargetNodeId = targetNodeId;
    }

    [JsonInclude]
    [JsonPropertyName("priority")]
    public int Priority { get; private set; }

    [JsonInclude]
    [JsonPropertyName("targetNodeId")]
    public Guid TargetNodeId { get; private set; }

    [JsonInclude]
    [JsonPropertyName("transitionRules")]
    public List<TransitionRule> TransitionRules { get; private set; } = new();

    public void AddTransitionRule(TransitionRule rule)
        => TransitionRules.Add(rule);

    public void SetTransitionRules(List<TransitionRule> rules)
        => TransitionRules = rules;
}