using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OperationsKnowledge.Common;
using OperationsKnowledge.Data;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;
using System;

namespace OperationsKnowledge.Tests;

public class OperationalSystemServiceTests
{
    [Fact]
    public async Task GetById_ReturnsSystem_WhenSystemExists()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
        };
        context.OperationalSystems.Add(system);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var result = await service.GetByIdAsync(system.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(system.Id, result.Id);
        Assert.Equal(system.Name, result.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenSystemDoesNotExist()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_PersistsSystem()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
        };
        OperationResult result = await service.CreateAsync(system);

        // Assert
        Assert.Equal(OperationResultStatus.Success, result.Status);
        var newSystem = await service.GetByIdAsync(system.Id);
        Assert.NotNull(newSystem);
        Assert.Equal(system.Id, newSystem.Id);
        Assert.Equal(system.Name, newSystem.Name);
    }

    [Fact]
    public async Task CreateWithoutOwner_Success()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
        };
        OperationResult result = await service.CreateAsync(system);

        // Assert
        Assert.Equal(OperationResultStatus.Success, result.Status);
    }

    [Fact]
    public async Task CreateWithOwner_Success()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var person = new Person()
        {
            Name = "Susan",
            Department = "Software Librarian",
            Email = "susan@organization.org",
            PhoneNumber = "555-555-1234"
        };
        context.People.Add(person);
        await context.SaveChangesAsync();

        var systemWithOwner = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
            OwnerId = person.Id,
            Owner = person
        };
        OperationResult result = await service.CreateAsync(systemWithOwner);

        // Assert
        Assert.Equal(OperationResultStatus.Success, result.Status);
    }

    [Fact]
    public async Task CreateWithNonexistentOwner_InvalidOwner()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var systemWithOwner = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
            OwnerId = 999
        };
        OperationResult result = await service.CreateAsync(systemWithOwner);

        // Assert
        Assert.Equal(OperationResultStatus.InvalidOwner, result.Status);
    }

    [Fact]
    public async Task Update_UpdatesExistingSystem()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
        };
        context.OperationalSystems.Add(system);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var updateSystem = new OperationalSystem()
        {
            Id = system.Id,
            Name = "Not Software Library",
            Status = "Not Operational",
            Description = "This system DOES NOT house a software library",
        };
        OperationResult result = await service.UpdateAsync(updateSystem);

        // Assert
        Assert.Equal(OperationResultStatus.Success, result.Status);
        var afterUpdate = await service.GetByIdAsync(updateSystem.Id);
        Assert.NotNull(afterUpdate);
        Assert.Equal(updateSystem.Id, afterUpdate.Id);
        Assert.Equal(updateSystem.Name, afterUpdate.Name);
        Assert.Equal(updateSystem.Status, afterUpdate.Status);
        Assert.Equal(updateSystem.Description, afterUpdate.Description);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenSystemDoesNotExist()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var updateSystem = new OperationalSystem()
        {
            Id = 999,
            Name = "Not Software Library",
            Status = "Not Operational",
            Description = "This system DOES NOT house a software library",
        };
        OperationResult result = await service.UpdateAsync(updateSystem);

        // Assert
        Assert.Equal(OperationResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdateWithoutOwner_Success()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
        };
        context.OperationalSystems.Add(system);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var updateSystem = new OperationalSystem()
        {
            Id = system.Id,
            Name = "Not Software Library",
            Status = "Not Operational",
            Description = "This system DOES NOT house a software library",
        };
        OperationResult result = await service.UpdateAsync(updateSystem);

        // Assert
        Assert.Equal(OperationResultStatus.Success, result.Status);
    }

    [Fact]
    public async Task UpdateWithOwner_Success()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var person = new Person()
        {
            Name = "Susan",
            Department = "Software Librarian",
            Email = "susan@organization.org",
            PhoneNumber = "555-555-1234"
        };
        context.People.Add(person);
        await context.SaveChangesAsync();

        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library"
        };
        context.OperationalSystems.Add(system);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var updateSystem = new OperationalSystem()
        {
            Id = system.Id,
            Name = "Not Software Library",
            Status = "Not Operational",
            Description = "This system DOES NOT house a software library",
            OwnerId = person.Id
        };
        OperationResult result = await service.UpdateAsync(updateSystem);

        // Assert
        Assert.Equal(OperationResultStatus.Success, result.Status);
    }

    [Fact]
    public async Task UpdateWithNonexistentOwner_InvalidOwner()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library"
        };
        context.OperationalSystems.Add(system);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var updateSystem = new OperationalSystem()
        {
            Id = system.Id,
            Name = "Not Software Library",
            Status = "Not Operational",
            Description = "This system DOES NOT house a software library",
            OwnerId = 999
        };
        OperationResult result = await service.UpdateAsync(updateSystem);

        // Assert
        Assert.Equal(OperationResultStatus.InvalidOwner, result.Status);
    }

    [Fact]
    public async Task UpdateNonexistentSystem_ReturnsNotFound()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var updateSystem = new OperationalSystem()
        {
            Id = 999,
            Name = "Not Software Library",
            Status = "Not Operational",
            Description = "This system DOES NOT house a software library"
        };
        OperationResult result = await service.UpdateAsync(updateSystem);

        // Assert
        Assert.Equal(OperationResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Delete_DeletesExistingSystem()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var system = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
        };
        context.OperationalSystems.Add(system);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var result = await service.DeleteAsync(system.Id);

        // Assert
        Assert.True(result);
        var removedSystem = await service.GetByIdAsync(system.Id);
        Assert.Null(removedSystem);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenSystemDoesNotExist()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var service = new OperationalSystemService(context);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetById_LoadsOwner_WhenSystemHasOwner()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var person = new Person()
        {
            Name = "Susan",
            Department = "Software Librarian",
            Email = "susan@organization.org",
            PhoneNumber = "555-555-1234"
        };
        context.People.Add(person);

        var systemWithoutOwner = new OperationalSystem()
        {
            Name = "Approved Hardware",
            Status = "Running",
            Description = "This system houses a list of approved Hardware",
        };
        context.OperationalSystems.Add(systemWithoutOwner);
        await context.SaveChangesAsync();

        var systemWithOwner = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
            OwnerId = person.Id,
            Owner = person
        };
        context.OperationalSystems.Add(systemWithOwner);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var ownedSystem = await service.GetByIdAsync(systemWithOwner.Id);

        // Assert
        Assert.NotNull(ownedSystem);
        Assert.NotNull(ownedSystem.OwnerId);
        Assert.NotNull(ownedSystem.Owner);
        Assert.Equal(person.Id, ownedSystem.Owner.Id);
        Assert.Equal(person.Name, ownedSystem.Owner.Name);
        Assert.Equal(person.Department, ownedSystem.Owner.Department);
        Assert.Equal(person.Email, ownedSystem.Owner.Email);
        Assert.Equal(person.PhoneNumber, ownedSystem.Owner.PhoneNumber);
    }

    [Fact]
    public async Task GetAll_ReturnsAllSystems()
    {
        // Arrange
        using var database = new TestDatabase();
        var context = database.Context;

        var system1 = new OperationalSystem()
        {
            Name = "Software Library",
            Status = "Operational",
            Description = "This system houses a software library",
        };
        context.OperationalSystems.Add(system1);

        var system2 = new OperationalSystem()
        {
            Name = "Approved Hardware",
            Status = "Running",
            Description = "This system houses a list of approved Hardware",
        };
        context.OperationalSystems.Add(system2);
        await context.SaveChangesAsync();

        var service = new OperationalSystemService(context);

        // Act
        var systems = await service.GetAllAsync();

        // Assert
        Assert.NotNull(systems);
        Assert.Equal(2, systems.Count);
        Assert.Contains(systems, s => s.Id == system1.Id);
        Assert.Contains(systems, s => s.Id == system2.Id);
    }
}
