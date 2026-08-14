using OperationsKnowledge.Models;
using System.ComponentModel.DataAnnotations;

namespace OperationsKnowledge.Dtos;

public class UpdateOperationalSystemRequest
{
    [Required(ErrorMessage = "Operational system must have a name.")]
    [StringLength(150, ErrorMessage = "Length limit of 150 characters.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Operational system must have a status.")]
    [StringLength(100, ErrorMessage = "Length limit of 100 characters.")]
    public string Status { get; set; } = "";

    [StringLength(1000, ErrorMessage = "Length limit of 1000 characters.")]
    public string Description { get; set; } = "";

    public int? OwnerId { get; set; }
}
