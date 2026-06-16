using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Workflows.Links;

namespace Parley.Workflows.Nodes.Factories;

public interface IParleyNodeFactory
{
    ParleyNode<ParleyLink> CreateNode(NodeConfig nodeConfig, WorkflowSchema workflowSchema);

    ParleyNodeOptionsValidator GetNodeValidator(string nodeType);

    void Preload();
}