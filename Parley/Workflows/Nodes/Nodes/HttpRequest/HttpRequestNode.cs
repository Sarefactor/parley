using Microsoft.Agents.AI.Workflows;
using Microsoft.AspNetCore.WebUtilities;
using Parley.Configuration.Attributes;
using Parley.Workflows.Links;
using Parley.Workflows.State;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.HttpRequest;

[ParleyNode]
[SendsMessage(typeof(ParleyLink))]
public class HttpRequestNode : ParleyNode<ParleyLink>
{
    private const string HttpClientName = "ParleyHttpNode";
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpRequestNode(ParleyNodeContext context,
                           IWorkflowStateManager workflowStateManager,
                           IHttpClientFactory httpClientFactory)
        : base(nameof(HttpRequestNode), context, workflowStateManager)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override string DialogType => nameof(HttpRequestNode);

    public override async ValueTask HandleAsync(ParleyLink message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = GetNodeOptions<HttpRequestNodeOptions>();

            var response = await SendRequest(options, context, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await context.SendMessageAsync(new ParleyLink((Guid)NodeConfig.SecondaryTransitionNode!), cancellationToken);
                return;
            }

            await ProcessResponse(response, options, context, cancellationToken);
        }
        catch(Exception ex)
        {
            var exMessage = ex.Message;
        }

        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
    }

    private async Task ProcessResponse(HttpResponseMessage response,
                                       HttpRequestNodeOptions options,
                                       IWorkflowContext context,
                                       CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadAsStringAsync();
        var root = JsonNode.Parse(payload);

        foreach (var mapping in options.ResponseMappings)
        {
            if (root is JsonObject obj
                && obj.TryGetPropertyValue("summary", out var summaryNode)
                && summaryNode?.GetValueKind() == JsonValueKind.String)
            {
                string summary = summaryNode.GetValue<string>();

                var workflowVariable = await WorkflowStateManager.GetWorkflowVariable(context, mapping.TargetKey, cancellationToken);

                workflowVariable.SetValue(summary);

                await WorkflowStateManager.SetWorkflowVariable(context, workflowVariable, cancellationToken);
            }
        }
    }

    private async Task<HttpResponseMessage> SendRequest(HttpRequestNodeOptions options,
                                                        IWorkflowContext context,
                                                        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(options.GetHttpMethod(),
                                             await BuildUri(options, context, cancellationToken));

        foreach (var requestHeader in options.Headers)
            request.Headers.TryAddWithoutValidation(requestHeader.Key, requestHeader.Value);

        var client = _httpClientFactory.CreateClient(HttpClientName);

        return await client.SendAsync(request, cancellationToken);
    }

    private async Task<Uri> BuildUri(HttpRequestNodeOptions options,
                                     IWorkflowContext context,
                                     CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string?>();

        foreach (var parameter in options.RequestParameters)
        {
            var variable = await WorkflowStateManager.GetWorkflowVariable(context, parameter.TargetKey, cancellationToken);

            if (variable.Value is not null)
                queryParams[parameter.ParameterName] = variable.Value.ToString();
        }

        return new Uri(QueryHelpers.AddQueryString(options.Url, queryParams));
    }

    public override WorkflowBuilder Configure(WorkflowBuilder builder,
                                          Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        builder.AddEdge<ParleyLink>(this,
                                    nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value,
                                    link => link?.TransitionNode == NodeConfig.PrimaryTransitionNode);

        builder.AddEdge<ParleyLink>(this,
                                    nodes.Single(x => x.Key == NodeConfig.SecondaryTransitionNode).Value,
                                    link => link?.TransitionNode == NodeConfig.SecondaryTransitionNode);

        return builder;
    }
}

[ExportTsClass]
public class HttpRequestNodeOptions : ParleyNodeOptions
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("method")]
    public HttpMethodType MethodType { get; set; } = HttpMethodType.Get;

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = new();

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = "application/json";

    [JsonPropertyName("timeoutSeconds")]
    public int TimeoutSeconds { get; set; } = 30;

    [JsonPropertyName("requestParameters")]
    public List<RequestParameters> RequestParameters { get; set; } = [];

    [JsonPropertyName("responseMappings")]
    public List<ResponseMapping> ResponseMappings { get; set; } = [];

    public HttpMethod GetHttpMethod()
        => MethodType switch
        {
            HttpMethodType.Get => HttpMethod.Get,
            HttpMethodType.Post => HttpMethod.Post,
            HttpMethodType.Put => HttpMethod.Put,
            _ => throw new NotImplementedException(),
        };
}