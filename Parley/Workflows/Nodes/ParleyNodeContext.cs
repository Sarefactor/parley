using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Workflows.Nodes;

public sealed record ParleyNodeContext(NodeConfig NodeConfig, WorkflowSchema WorkflowSchema);