using System.ComponentModel.DataAnnotations;

namespace MiVoluntariadoAPI.DTOs.Auth
{
    public class RegisterVoluntarioDto
    {
        [Required] public required string Nombre { get; set; }
        [Required] public required string Apellido { get; set; }
        [Required] [EmailAddress] public required string Email { get; set; }
        [Required] [MinLength(6)] public required string Password { get; set; }
    }
}