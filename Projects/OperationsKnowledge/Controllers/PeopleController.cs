using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Dtos;
using OperationsKnowledge.Mappings;
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
        var people = await _service.GetAllAsync();
        return people.Select(ToResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PersonResponse>> GetPerson(int id)
    {
        var system = await _service.GetByIdAsync(id);
        if (system == null) { return NotFound(); }
        return Ok(ToResponse(system));
    }

    [HttpGet("{id}/OwnedSystems")]
    public async Task<ActionResult<IEnumerable<OperationalSystemResponse>>> OwnedSystemsAsync(int id)
    {
        var systems = await _service.GetOwnedSystemsAsync(id);
        return Ok(systems.Select(OperationalSystemMapper.ToResponse));
    }

    [HttpPost]
    public async Task<ActionResult<Person>> CreatePersonAsync(CreatePersonRequest request)
    {
        var person = new Person
        {
            Name = request.Name,
            Department = request.Department,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };
        await _service.CreateAsync(person);
        return CreatedAtAction(
            nameof(GetPerson), // throws "No route matches the supplied values." when using name "GetPersonAsync"
            new { id = person.Id },
            ToResponse(person));
    }

    [HttpPut]
    public async Task<ActionResult<Person>> UpdatePersonAsync(int id, UpdatePersonRequest request)
    {
        var person = new Person
        {
            Id = id,
            Name = request.Name,
            Department = request.Department,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };
        bool updated = await _service.UpdateAsync(person);
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
