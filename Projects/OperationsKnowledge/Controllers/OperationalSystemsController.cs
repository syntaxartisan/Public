using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Dtos;
using OperationsKnowledge.Mappings;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;

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
        await _service.CreateAsync(system);
        return CreatedAtAction(
            nameof(GetOperationalSystem), // throws "No route matches the supplied values." when using name "GetOperationalSystemAsync"
            new { id = system.Id },
            OperationalSystemMapper.ToResponse(system));
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(OperationalSystem), StatusCodes.Status200OK)]
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
            OwnerId = request.OwnerId,
            Owner = null
        };
        bool updated = await _service.UpdateAsync(system);
        if (!updated) { return NotFound(); }
        return Ok(updated);
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
