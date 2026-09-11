using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

    // Failing test: Returning NotFound because there's no data
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
}
