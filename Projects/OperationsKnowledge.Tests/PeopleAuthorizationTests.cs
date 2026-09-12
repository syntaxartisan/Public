using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OperationsKnowledge.Tests;

public class PeopleAuthorizationTests
{
    [Fact]
    public async Task GetPerson_ReturnsUnauthorized_WhenNoTokenIsProvided()
    {
        // Arrange
        //var factory = new WebApplicationFactory<Program>();
        var factory = new CustomWebApplicationFactory()
        {
            User = TestUser.Anonymous
        };
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/people/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPerson_ReturnsOk_WhenAuthenticated()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory()
        {
            User = TestUser.User
        };
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/people/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeletePerson_ReturnsUnauthorized_WhenNoTokenIsProvided()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory()
        {
            User = TestUser.Anonymous
        };
        var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/people/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletePerson_ReturnsForbidden_WhenUserIsNotAdministrator()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory()
        {
            User = TestUser.User
        };
        var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/people/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeletePerson_ReturnsNoContent_WhenAdministratorDeletesPerson()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory()
        {
            User = TestUser.Administrator
        };
        var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/people/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CreatePerson_ReturnsBadRequest_WhenNameIsMissing()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory()
        {
            User = TestUser.User
        };
        var client = factory.CreateClient();

        var request = new
        {
            // Name intentionally missing
            Department = "Testing",
            Email = "test@example.com",
            PhoneNumber = "555-555-0000"
        };

        // Act
        var response = await client.PostAsJsonAsync("/people", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains("Name", problem.Errors.Keys);
        Assert.Contains("Person must have a Name", problem.Errors["Name"]);
    }
}
