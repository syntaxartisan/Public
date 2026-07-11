using Microsoft.AspNetCore.Mvc;
using OperationsKnowledge.Models;
using OperationsKnowledge.Services;

namespace OperationsKnowledge.Controllers;

[ApiController]
[Route("[controller]")]
public class OperationalSystemsController : ControllerBase
{
    private readonly IOperationalSystemService _service;

    public OperationalSystemsController(IOperationalSystemService service)
    {  _service = service; }

    [HttpGet]
    public IEnumerable<OperationalSystem> GetOperationalSystems()
    {
        //return new List<OperationalSystem>
        //{
        //    new()
        //    {
        //        Id = 1,
        //        Name = "Payroll",
        //        Status = "Active",
        //        Description = "Processes employee payroll."
        //    },
        //    new()
        //    {
        //        Id = 2,
        //        Name = "Monitoring",
        //        Status = "Active",
        //        Description = "Infrastructure monitoring."
        //    }
        //};
        return _service.GetAll();
    }
    [HttpGet("{id}")]
    public ActionResult<OperationalSystem> GetOperationalSystem(int id)
    {
        //var system = GetOperationalSystems().FirstOrDefault(s => s.Id == id);
        var system = _service.GetById(id);
        if (system == null) { return NotFound(); }
        return system;
    }
}
