using Microsoft.Agents.AI;

namespace Parley.Providers;

public interface IAgentProvider
{
    Task<AIAgent> CreateParleyAgent();
}