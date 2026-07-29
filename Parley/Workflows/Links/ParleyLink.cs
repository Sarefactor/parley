namespace Parley.Workflows.Links;

public class ParleyLink
{
    public ParleyLink(Guid transitionNode)
    {
        TransitionNode = transitionNode;
    }

    public Guid TransitionNode { get; set; }
    public string? LinkMessage { get; set; } = string.Empty;    
}