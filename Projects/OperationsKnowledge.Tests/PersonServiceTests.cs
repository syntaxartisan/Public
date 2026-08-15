using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OperationsKnowledge.Data;
using OperationsKnowledge.Services;
using OperationsKnowledge.Models;

namespace OperationsKnowledge.Tests;

public class PersonServiceTests
{
    [Fact]
    public async Task DeletePerson_UnassignsOwnedSystem()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var susan = new Person
        {
            Name = "Susan",
            Department = "IT",
            Email = "susan@organization.org"
        };
        context.People.Add(susan);
        await context.SaveChangesAsync();

        var system = new OperationalSystem
        {
            Name = "Software Library",
            Status = "Active",
            Description = "Library of software titles",
            OwnerId = susan.Id
        };
        context.OperationalSystems.Add(system);
        await context.SaveChangesAsync();

        var service = new PersonService(context);

        // Act
        var result = await service.DeleteAsync(susan.Id);

        // Assert
        Assert.True(result);
        var deletedPerson = await context.People.FirstOrDefaultAsync(p => p.Id == susan.Id);
        var remainingSystem = await context.OperationalSystems.FirstOrDefaultAsync(s => s.Id == system.Id);

        Assert.Null(deletedPerson);
        Assert.NotNull(remainingSystem);
        Assert.Null(remainingSystem.OwnerId);
    }

    [Fact]
    public async Task GetOwnedSystems_ReturnsSystemsOwnedByPerson()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var susan = new Person
        {
            Name = "Susan",
            Department = "IT",
            Email = "susan@organization.org"
        };
        context.People.Add(susan);
        await context.SaveChangesAsync();

        var system_with_owner = new OperationalSystem
        {
            Name = "Software Library",
            Status = "Active",
            Description = "Library of software titles",
            OwnerId = susan.Id
        };
        context.OperationalSystems.Add(system_with_owner);

        var system_without_owner = new OperationalSystem
        {
            Name = "Approved Hardware",
            Status = "Active",
            Description = "Library of approved hardware",
            OwnerId = null
        };
        context.OperationalSystems.Add(system_without_owner);
        await context.SaveChangesAsync();

        var service = new PersonService(context);

        // Act
        var result = await service.GetOwnedSystemsAsync(susan.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal(system_with_owner.Id, result[0].Id);
        Assert.Equal(susan.Id, result[0].Owner!.Id);
        Assert.NotNull(result[0].Owner);
        Assert.Equal(susan.Id, result[0].OwnerId);
    }
}