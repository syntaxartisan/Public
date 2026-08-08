using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Dtos;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;

namespace OperationsKnowledge.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _service;

    public PeopleController(IPersonService service)
    { _service = service; }

    [HttpGet]
    public async Task<IEnumerable<PersonResponse>> PeopleAsync()
    {
        var systems = await _service.GetAllAsync();
        return systems.Select(ToResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PersonResponse>> GetPerson(int id)
    {
        var system = await _service.GetByIdAsync(id);
        if (system == null) { return NotFound(); }
        return Ok(ToResponse(system));
    }

    [HttpPost]
    public async Task<ActionResult<Person>> CreatePersonAsync(CreatePersonRequest request)
    {
        var system = new Person
        {
            Name = request.Name,
            Department = request.Department,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };
        await _service.CreateAsync(system);
        return CreatedAtAction(
            nameof(GetPerson), // throws "No route matches the supplied values." when using name "GetPersonAsync"
            new { id = system.Id },
            ToResponse(system));
    }

    [HttpPut]
    public async Task<ActionResult<Person>> UpdatePersonAsync(int id, UpdatePersonRequest request)
    {
        var system = new Person
        {
            Id = id,
            Name = request.Name,
            Department = request.Department,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };
        bool updated = await _service.UpdateAsync(system);
        if (!updated) { return NotFound(); }
        return Ok(updated);
    }

    [HttpDelete]
    public async Task<ActionResult> DeletePersonAsync(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        if (!deleted) { return NotFound(); }
        return NoContent();
    }

    private static PersonResponse ToResponse(Person p)
    {
        return new PersonResponse
        {
            Id = p.Id,
            Name = p.Name,
            Department = p.Department,
            Email = p.Email,
            PhoneNumber = p.PhoneNumber
        };
    }
}
