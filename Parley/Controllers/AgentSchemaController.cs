using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Parley.Configuration.Exceptions;
using Parley.Core.DataAccess.Factories;
using Parley.Core.Services;
using Parley.Dtos.Schema;
using Parley.Dtos.Search;
using Parley.Dtos.Validation;
using Parley.Providers;

namespace Parley.Controllers;

[Route("api/parley/agentschemas")]
[ApiController]
public class AgentSchemaController : ControllerBase
{
    private readonly ISchemaFactory _schemaFactory;
    private readonly IAgentSchemaRegistry _agentSchemaRegistry;
    private readonly IAgentSchemaProvider _schemaProvider;

    public AgentSchemaController(ISchemaFactory schemaFactory,
                                 IAgentSchemaRegistry agentSchemaRegistry,
                                 IAgentSchemaProvider agentSchemaProvider)
    {
        _schemaFactory = schemaFactory;
        _agentSchemaRegistry = agentSchemaRegistry;
        _schemaProvider = agentSchemaProvider;
    }

    [HttpGet]
    [Route("search")]
    [ProducesResponseType(typeof(SearchResultDto<SchemaSearchItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] int skip, [FromQuery] int take)
    {
        var result = await _agentSchemaRegistry.Search(skip, take);
        return Ok(result);
    }

    [HttpGet]
    [Route("get")]
    [ProducesResponseType(typeof(AgentSchemaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid agentSchemaId)
    {
        var result = await _agentSchemaRegistry.Get(agentSchemaId);
        return Ok(result);
    }

    [HttpPost]
    [Route("upsert")]
    [ParleyExceptionFilter]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ParleyValidationContextDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Upsert([FromBody] AgentSchemaDto agentSchemaDto)
    {
        await _schemaFactory.Upsert(agentSchemaDto);
        return Ok();
    }

    [HttpDelete]
    [Route("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromQuery] Guid agentSchemaId)
    {
        await _agentSchemaRegistry.Delete(agentSchemaId);
        return Ok();
    }

    [HttpPost]
    [Route("setActiveSchema")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SetActiveSchema([FromQuery] Guid agentSchemaId)
    {
        await _schemaProvider.SetActiveSchema(agentSchemaId);
        return Ok();
    }
}