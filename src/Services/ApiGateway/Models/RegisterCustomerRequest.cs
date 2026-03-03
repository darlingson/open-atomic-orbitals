using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models;

public class RegisterCustomerRequest
{
    [Required]
    public string FullName { get; set; } = default!;

    [Required]
    public string NationalId { get; set; } = default!;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = default!;

    [Required]
    public DateOnly DateOfBirth { get; set; }
}