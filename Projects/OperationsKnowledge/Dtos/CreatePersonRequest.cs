using System.ComponentModel.DataAnnotations;
using OperationsKnowledge.Models;

namespace OperationsKnowledge.Dtos;

public class CreatePersonRequest
{
    [Required(ErrorMessage = "Person must have a Name")]
    [StringLength(150, ErrorMessage = "Length limit of 150 characters")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Person must have a Department")]
    [StringLength(100, ErrorMessage = "Length limit of 100 characters")]
    public string Department { get; set; } = "";

    public string Email { get; set; } = "";

    public string? PhoneNumber { get; set; }
}
