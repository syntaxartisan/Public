using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OperationsKnowledge.Data;
using OperationsKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsKnowledge.Tests;

public enum TestUser
{
    Anonymous,
    User,
    Administrator
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
    public TestUser User { get; set; } = TestUser.Anonymous;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove SQL database binding
            services.Remove(
                services.Single(d =>
                d.ServiceType == typeof(DbContextOptions<OperationalSystemContext>)));

            // Bind to SQLite instead
            services.AddDbContext<OperationalSystemContext>(options =>
            options.UseSqlite(_connection));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(
                "Test",
                options =>
                {
                    options.Role = User switch
                    {
                        TestUser.User => "User",
                        TestUser.Administrator => "Administrator",
                        _ => null
                    };
                }
            );

            // Create SQLite database
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OperationalSystemContext>();
                context.Database.EnsureCreated();
                var person = new Person
                {
                    Name = "Susan",
                    Department = "IT",
                    Email = "susan@organization.org",
                    PhoneNumber = "555-555-1234"
                };
                context.People.Add(person);
                context.SaveChanges();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
