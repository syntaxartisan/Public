using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Models;

namespace OperationsKnowledge.Controllers;

[ApiController]
[Route("[controller]")]
public class OperationalSystemsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<OperationalSystem> GetOperationalSystems()
    {
        return new List<OperationalSystem>
        {
            new()
            {
                Id = 1,
                Name = "Payroll",
                Status = "Active",
                Description = "Processes employee payroll."
            },
            new()
            {
                Id = 2,
                Name = "Monitoring",
                Status = "Active",
                Description = "Infrastructure monitoring."
            }
        };
    }
    [HttpGet("{id}")]
    public ActionResult<OperationalSystem> GetOperationalSystem(int id)
    {
        var system = GetOperationalSystems().FirstOrDefault(s => s.Id == id);
        if (system == null) { return NotFound(); }
        return system;
    }
}
