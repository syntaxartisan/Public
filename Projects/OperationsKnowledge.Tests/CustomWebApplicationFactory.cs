using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
    public TestUser User { get; set; } = TestUser.Anonymous;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
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
                });
        });
    }
}
