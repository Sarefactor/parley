using Microsoft.Extensions.DependencyInjection;
using Parley.Configuration.Attributes;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Workflows.Links;

namespace Parley.Workflows.Nodes.Factories;

public class ParleyNodeFactory : IParleyNodeFactory
{
    private IServiceProvider _serviceProvider { get; set; }

    public ParleyNodeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private readonly Dictionary<string, ObjectFactory> NodeFactories = new();

    private readonly Dictionary<string, ParleyNodeOptionsValidator> NodeOptionValidators = new();

    public ParleyNode<ParleyLink> CreateNode(NodeConfig nodeConfig, WorkflowSchema workflowSchema)
    {
        if (!NodeFactories.Any(x => x.Key == nodeConfig.NodeType))
            throw new ArgumentException($"Unable to construct a node for the type: {nodeConfig.NodeType}");

        return (ParleyNode<ParleyLink>)NodeFactories[nodeConfig.NodeType](_serviceProvider,
                                                                          [new ParleyNodeContext(nodeConfig, workflowSchema)]);
    }

    public ParleyNodeOptionsValidator GetNodeValidator(string nodeType)
        => NodeOptionValidators.Single(x => x.Key == nodeType).Value;

    public void Preload()
    {
        LoadNodes();
        LoadNodeValidators();
    }

    private void LoadNodes()
    {
        var baseType = typeof(ParleyNode<ParleyLink>);

        var types = AppDomain.CurrentDomain.GetAssemblies()
                                           .SelectMany(a => a.GetTypes())
                                           .Where(t => !t.IsAbstract
                                                       && t.IsDefined(typeof(ParleyNodeAttribute), inherit: false)
                                                       && baseType.IsAssignableFrom(t));

        foreach (var type in types)
            NodeFactories[type.Name] = ActivatorUtilities.CreateFactory(type, [typeof(ParleyNodeContext)]);
    }

    private void LoadNodeValidators()
    {
        var baseType = typeof(ParleyNodeOptionsValidator);

        var types = AppDomain.CurrentDomain.GetAssemblies()
                                           .SelectMany(a => a.GetTypes())
                                           .Where(t => !t.IsAbstract
                                                  && t.IsDefined(typeof(ParleyNodeValidatorAttribute), inherit: false)
                                                  && baseType.IsAssignableFrom(t));

        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is not ParleyNodeOptionsValidator optionsValidator)
                continue;

            NodeOptionValidators.Add(optionsValidator.NodeType, optionsValidator);
        }
    }
}