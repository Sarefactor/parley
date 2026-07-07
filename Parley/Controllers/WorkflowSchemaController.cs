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

[Route("api/parley/workflowschemas")]
[ApiController]
public class WorkflowSchemaController : ControllerBase
{
    private readonly ISchemaFactory _schemaFactory;
    private readonly IWorkflowSchemaRegistry _workflowSchemaRegistry;

    public WorkflowSchemaController(ISchemaFactory schemaFactory,
                                    IWorkflowSchemaRegistry workflowSchemaRegistry)
    {
        _schemaFactory = schemaFactory;
        _workflowSchemaRegistry = workflowSchemaRegistry;
    }

    [HttpGet]
    [Route("search")]
    [ProducesResponseType(typeof(SearchResultDto<SchemaSearchItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] int skip, [FromQuery] int take)
    {
        var result = await _workflowSchemaRegistry.Search(skip, take);
        return Ok(result);
    }

    [HttpGet]
    [Route("get")]
    [ProducesResponseType(typeof(AgentSchemaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid workflowSchemaId)
    {
        var result = await _workflowSchemaRegistry.Get(workflowSchemaId);
        return Ok(result);
    }

    [HttpPost]
    [Route("upsert")]
    [ParleyExceptionFilter]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ParleyValidationContextDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Upsert([FromBody] WorkflowSchemaDto workflowSchemaDto)
    {
        await _schemaFactory.Upsert(workflowSchemaDto);
        return Ok();
    }

    [HttpDelete]
    [Route("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromQuery] Guid workflowSchemaId)
    {
        await _workflowSchemaRegistry.Delete(workflowSchemaId);
        return Ok();
    }
}