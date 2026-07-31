using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Dtos;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;

namespace OperationsKnowledge.Controllers;

[ApiController]
[Route("[controller]")]
public class OperationalSystemsController : ControllerBase
{
    private readonly IOperationalSystemService _service;

    public OperationalSystemsController(IOperationalSystemService service)
    {  _service = service; }

    [HttpGet]
    public async Task<IEnumerable<OperationalSystemResponse>> GetOperationalSystemsAsync()
    {
        var systems = await _service.GetAllAsync();
        return systems.Select(ToResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OperationalSystemResponse>> GetOperationalSystemAsync(int id)
    {
        var system = await _service.GetByIdAsync(id);
        if (system == null) { return NotFound(); }
        return Ok(ToResponse(system));
    }

    [HttpPost]
    public async Task<ActionResult<OperationalSystem>> CreateOperationalSystemAsync(CreateOperationalSystemRequest request)
    {
        var system = new OperationalSystem
        {
            Name = request.Name,
            Status = request.Status,
            Description = request.Description,
            Owner = request.Owner
        };
        await _service.CreateAsync(system);
        return CreatedAtAction(
            nameof(GetOperationalSystemAsync),
            new { id = system.Id },
            system);
    }

    [HttpPut]
    public async Task<ActionResult<OperationalSystem>> UpdateOperationalSystemAsync(int id, UpdateOperationalSystemRequest request)
    {
        var system = new OperationalSystem
        {
            Id = id,
            Name = request.Name,
            Status = request.Status,
            Description = request.Description,
            Owner = request.Owner
        };
        bool updated = await _service.UpdateAsync(system);
        if (!updated) { return NotFound(); }
        return Ok(updated);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteOperationalSystemAsync(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        if (!deleted) { return NotFound(); }
        return NoContent();
    }

    private static OperationalSystemResponse ToResponse(OperationalSystem system)
    {
        return new OperationalSystemResponse
        {
            Id = system.Id,
            Name = system.Name,
            Status = system.Status,
            Description = system.Description,
            Owner = system.Owner
        };
    }
}
