using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;
using OperationsKnowledge.Dtos;

namespace OperationsKnowledge.Controllers;

[ApiController]
[Route("[controller]")]
public class OperationalSystemsController : ControllerBase
{
    private readonly IOperationalSystemService _service;

    public OperationalSystemsController(IOperationalSystemService service)
    {  _service = service; }

    [HttpGet]
    public IEnumerable<OperationalSystem> GetOperationalSystems()
    {
        return _service.GetAll();
    }
    [HttpGet("{id}")]
    public ActionResult<OperationalSystem> GetOperationalSystem(int id)
    {
        var system = _service.GetById(id);
        if (system == null) { return NotFound(); }
        return system;
    }
    [HttpPost]
    public ActionResult<OperationalSystem> CreateOperationalSystem(CreateOperationalSystemRequest request)
    {
        var system = new OperationalSystem
        {
            Name = request.Name,
            Status = request.Status,
            Description = request.Description,
            Owner = request.Owner
        };
        var created = _service.Create(system);
        return CreatedAtAction(
            nameof(GetOperationalSystem),
            new { id = created.Id },
            created);
    }
    [HttpPut]
    public ActionResult<OperationalSystem> UpdateOperationalSystem(int id, UpdateOperationalSystemRequest request)
    {
        var system = new OperationalSystem
        {
            Id = id,
            Name = request.Name,
            Status = request.Status,
            Description = request.Description,
            Owner = request.Owner
        };
        var updated = _service.Update(system);
        if (updated == null) { return NotFound(); }
        return Ok(updated);
    }
    [HttpDelete]
    public ActionResult DeleteOperationalSystem(int id)
    {
        bool deleted = _service.Delete(id);
        if (!deleted) { return NotFound(); }
        return NoContent();
    }
}
