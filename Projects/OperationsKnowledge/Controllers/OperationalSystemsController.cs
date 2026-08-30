using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Common;
using OperationsKnowledge.Dtos;
using OperationsKnowledge.Mappings;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OperationsKnowledge.Controllers;

[ApiController]
[Route("operational-systems")]
public class OperationalSystemsController : ControllerBase
{
    private readonly IOperationalSystemService _service;

    public OperationalSystemsController(IOperationalSystemService service)
    {  _service = service; }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OperationalSystemResponse>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<OperationalSystemResponse>> GetOperationalSystemsAsync()
    {
        var systems = await _service.GetAllAsync();
        return systems.Select(OperationalSystemMapper.ToResponse);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OperationalSystemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationalSystemResponse>> GetOperationalSystem(int id)
    {
        var system = await _service.GetByIdAsync(id);
        if (system == null) { return NotFound(); }
        return Ok(OperationalSystemMapper.ToResponse(system));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OperationalSystemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OperationalSystemResponse>> CreateOperationalSystemAsync(CreateOperationalSystemRequest request)
    {
        var system = new OperationalSystem
        {
            Name = request.Name,
            Status = request.Status,
            Description = request.Description,
            OwnerId = request.OwnerId
        };
        var result = await _service.CreateAsync(system);
        return result.Status switch
        {
            OperationResultStatus.Success => CreatedAtAction(
                nameof(GetOperationalSystem), // throws "No route matches the supplied values." when using name "GetOperationalSystemAsync"
                new { id = system.Id },
                OperationalSystemMapper.ToResponse(system)),
            OperationResultStatus.InvalidOwner => BadRequest("The specified owner does not exist."),
            _ => StatusCode(500)
        };
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(OperationalSystem), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationalSystem>> UpdateOperationalSystemAsync(int id, UpdateOperationalSystemRequest request)
    {
        var system = new OperationalSystem
        {
            Id = id,
            Name = request.Name,
            Status = request.Status,
            Description = request.Description,
            OwnerId = request.OwnerId
        };
        var result = await _service.UpdateAsync(system);
        return result.Status switch
        {
            OperationResultStatus.Success => NoContent(),
            OperationResultStatus.NotFound => NotFound(),
            OperationResultStatus.InvalidOwner => BadRequest("The specified owner does not exist."),
            _ => StatusCode(500)
        };
    }

    [HttpDelete]
    [Authorize(Policy = "AdministratorOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteOperationalSystemAsync(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        if (!deleted) { return NotFound(); }
        return NoContent();
    }
}
