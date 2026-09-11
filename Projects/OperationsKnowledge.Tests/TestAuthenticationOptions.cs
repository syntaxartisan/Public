using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsKnowledge.Tests;

public class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    public string? Role { get; set; }
}
