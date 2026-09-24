using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
    [Fact]
    public async Task GetPerson_ReturnsOk_WhenPersonExists()
    {
        // Arrange
        var person = new Person
        {
            Id = 1,
            Name = "Susan",
            Department = "IT",
            Email = "susan@organization.org"
        };
        var service = new Mock<IPersonService>();
        service.Setup(s => s.GetByIdAsync(person.Id)).ReturnsAsync(person);
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.GetPerson(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PersonResponse>(okResult.Value);
        Assert.Equal(person.Id, response.Id);
        Assert.Equal(person.Name, response.Name);
        Assert.Equal(person.Department, response.Department);
    }

    [Fact]
    public async Task GetPerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var service = new Mock<IPersonService>();
        service.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Person?)null);
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.GetPerson(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task People_ReturnsPeople()
    {
        // Arrange
        var people = new List<Person>()
        {
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
        };
        var service = new Mock<IPersonService>();
        service.Setup(s => s.GetAllAsync()).ReturnsAsync(people);
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.GetPeopleAsync();

        // Assert
        var responses = result.ToList();
        Assert.Equal(2, responses.Count);
        for (int i = 0; i < responses.Count; i++)
        {
            Assert.Equal(people[i].Id, responses[i].Id);
            Assert.Equal(people[i].Name, responses[i].Name);
            Assert.Equal(people[i].Department, responses[i].Department);
        }
    }

    [Fact]
    public async Task OwnedSystems_ReturnsSystems_WhenPersonExists()
    {
        // Arrange
        var systems = new List<OperationalSystem>
        {
            new OperationalSystem
            {
                Name = "Software Library",
                Status = "Operational",
                Description = "This system houses a software library"
            },
            new OperationalSystem
            {
                Name = "Approved Hardware",
                Status = "Running",
                Description = "This system houses a list of approved Hardware",
            }
        };
        var person = new Person
        {
            Id = 1,
            Name = "Susan",
            Department = "IT",
            Email = "susan@organization.org"
        };
        var service = new Mock<IPersonService>();
        service.Setup(s => s.GetByIdAsync(person.Id)).ReturnsAsync(person);
        service.Setup(s => s.GetOwnedSystemsAsync(person.Id)).ReturnsAsync(systems);
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.OwnedSystemsAsync(person.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responses = Assert.IsAssignableFrom<IEnumerable<OperationalSystemResponse>>(okResult.Value).ToList();
        Assert.Equal(2, responses.Count);
        for (int i = 0; i < responses.Count; i++)
        {
            Assert.Equal(systems[i].Name, responses[i].Name);
            Assert.Equal(systems[i].Status, responses[i].Status);
            Assert.Equal(systems[i].Description, responses[i].Description);
        }
    }

    [Fact]
    public async Task OwnedSystems_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var service = new Mock<IPersonService>();
        service.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Person?)null);
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.OwnedSystemsAsync(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreatePerson_ReturnsCreated_WhenPersonIsCreated()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            Name = "Susan",
            Department = "IT",
            Email = "susan@organization.org",
            PhoneNumber = "555-555-1234"
        };
        var service = new Mock<IPersonService>();
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.CreatePersonAsync(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        var response = Assert.IsType<PersonResponse>(createdResult.Value);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.Department, response.Department);
        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.PhoneNumber, response.PhoneNumber);
        service.Verify(
            s => s.CreateAsync(It.Is<Person>(p =>
                p.Name == request.Name &&
                p.Department == request.Department &&
                p.Email == request.Email &&
                p.PhoneNumber == request.PhoneNumber)),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePerson_ReturnsOk_WhenPersonIsUpdated()
    {
        // Arrange
        var service = new Mock<IPersonService>();
        service.Setup(s => s.UpdateAsync(It.IsAny<Person>()))
            .ReturnsAsync(new OperationResult(OperationResultStatus.Success));
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.UpdatePersonAsync(999, new UpdatePersonRequest());

        // Assert
        var updatedResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, updatedResult.StatusCode);
        var person = Assert.IsType<PersonResponse>(updatedResult.Value);
        Assert.Equal(999, person.Id);
    }

    [Fact]
    public async Task UpdatePerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var service = new Mock<IPersonService>();
        service.Setup(s => s.UpdateAsync(It.IsAny<Person>()))
            .ReturnsAsync(new OperationResult(OperationResultStatus.NotFound));
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.UpdatePersonAsync(999, new UpdatePersonRequest());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeletePerson_ReturnsNoContent_WhenPersonIsDeleted()
    {
        // Arrange
        var service = new Mock<IPersonService>();
        service.Setup(s => s.DeleteAsync(999)).ReturnsAsync(true);
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.DeletePersonAsync(999);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var service = new Mock<IPersonService>();
        service.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);
        var controller = new PeopleController(service.Object);

        // Act
        var result = await controller.DeletePersonAsync(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
