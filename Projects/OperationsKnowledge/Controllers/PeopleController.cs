using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Common;
using OperationsKnowledge.Dtos;
using OperationsKnowledge.Mappings;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;

namespace OperationsKnowledge.Controllers;

[ApiController]
[Route("people")]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _service;

    public PeopleController(IPersonService service)
    { _service = service; }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonResponse>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<PersonResponse>> PeopleAsync()
    {
        var people = await _service.GetAllAsync();
        return people.Select(ToResponse);
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponse>> GetPerson(int id)
    {
        var person = await _service.GetByIdAsync(id);
        if (person == null) { return NotFound(); }
        return Ok(ToResponse(person));
    }

    [HttpGet("{id}/owned-systems")]
    [ProducesResponseType(typeof(IEnumerable<OperationalSystemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<OperationalSystemResponse>>> OwnedSystemsAsync(int id)
    {
        var person = await _service.GetByIdAsync(id);
        if (person == null) { return NotFound(); }
        var systems = await _service.GetOwnedSystemsAsync(id);
        return Ok(systems.Select(OperationalSystemMapper.ToResponse));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PersonResponse>> CreatePersonAsync(CreatePersonRequest request)
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

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponse>> UpdatePersonAsync(int id, UpdatePersonRequest request)
    {
        var person = new Person
        {
            Id = id,
            Name = request.Name,
            Department = request.Department,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };
        OperationResult result = await _service.UpdateAsync(person);
        return result.Status switch
        {
            OperationResultStatus.Success => Ok(ToResponse(person)),
            OperationResultStatus.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdministratorOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
