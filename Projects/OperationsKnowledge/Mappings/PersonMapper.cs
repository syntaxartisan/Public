using OperationsKnowledge.Dtos;
using OperationsKnowledge.Models;

namespace OperationsKnowledge.Mappings;

public static class PersonMapper
{
    public static PersonResponse ToResponse(Person person)
    {
        return new PersonResponse
        {
            Id = person.Id,
            Name = person.Name,
            Department = person.Department,
            Email = person.Email,
            PhoneNumber = person.PhoneNumber
        };
    }
}
