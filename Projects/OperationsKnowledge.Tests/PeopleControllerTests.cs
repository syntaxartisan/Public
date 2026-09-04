using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Common;
using OperationsKnowledge.Controllers;
using OperationsKnowledge.Dtos;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsKnowledge.Tests;

public class PeopleControllerTests
{
    private class FakePersonService : IPersonService
    {
        public Person? PersonToReturn { get; set; }
        public IReadOnlyList<Person> PeopleToReturn { get; set; } = [];

        public Task<IReadOnlyList<Person>> GetAllAsync()
        {
            return Task.FromResult(PeopleToReturn);
        }

        public Task<Person> GetByIdAsync(int id)
        {
            return Task.FromResult(PersonToReturn);
        }

        public Task<IReadOnlyList<OperationalSystem>> GetOwnedSystemsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Person p)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdateAsync(Person p)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public async Task GetPerson_ReturnsOk_WhenPersonExists()
    {
        // Arrange
        var service = new FakePersonService();
        service.PersonToReturn = new Person
        {
            Id = 1,
            Name = "Susan",
            Department = "IT",
            Email = "susan@organization.org"
        };
        var controller = new PeopleController(service);

        // Act
        var result = await controller.GetPerson(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PersonResponse>(okResult.Value);
        Assert.Equal(service.PersonToReturn.Id, response.Id);
        Assert.Equal(service.PersonToReturn.Name, response.Name);
        Assert.Equal(service.PersonToReturn.Department, response.Department);
    }

    [Fact]
    public async Task GetPerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var service = new FakePersonService();
        service.PersonToReturn = null;
        var controller = new PeopleController(service);

        // Act
        var result = await controller.GetPerson(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

/* Incomplete
    [Fact]
    public async Task People_ReturnsPeople()
    {
        // Arrange
        var service = new FakePersonService();
        service.PeopleToReturn =
        [
            new Person
            {
                Id = 1,
                Name = "Susan",
                Department = "IT",
                Email = "susan@organization.org"
            },
            new Person
            {
                Id = 2,
                Name = "Bob",
                Department = "Engineering",
                Email = "bob@organization.org"
            }
        ];
        var controller = new PeopleController(service);

        // Act
        var result = await controller.PeopleAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PersonResponse>(okResult.Value);
        Assert.Equal(service.PersonToReturn.Id, response.Id);
        Assert.Equal(service.PersonToReturn.Name, response.Name);
        Assert.Equal(service.PersonToReturn.Department, response.Department);
    }
*/
}
