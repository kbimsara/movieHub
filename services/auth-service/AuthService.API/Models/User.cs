using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.API.Models;

[Table("Users")]
public class User
{
    [Key]
    [Column("Id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("Username")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Column("Email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("RefreshToken")]
    public string? RefreshToken { get; set; }

    [Column("RefreshTokenExpiry")]
    public DateTime? RefreshTokenExpiry { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("IsActive")]
    public bool IsActive { get; set; } = true;
}
