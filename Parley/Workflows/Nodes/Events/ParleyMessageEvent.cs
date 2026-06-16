using Microsoft.Agents.AI.Workflows;

namespace Parley.Workflows.Nodes.Events;

public class ParleyMessageEvent : WorkflowEvent
{
    public ParleyMessageEvent(string message)
    {
        Message = message;
    }

    public string Message { get; private set; }
}