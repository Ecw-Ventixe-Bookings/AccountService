

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(100)]
    public string? City { get; set; }
}
