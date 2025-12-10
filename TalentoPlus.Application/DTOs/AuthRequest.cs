using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Application.DTOs;

public class AuthRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Documento { get; set; } = string.Empty; // Using Documento as password for initial setup per requirements or simple check
}
