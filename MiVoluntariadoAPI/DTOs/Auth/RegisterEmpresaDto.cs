using System.ComponentModel.DataAnnotations;

namespace MiVoluntariadoAPI.DTOs.Auth
{
    public class RegisterEmpresaDto
    {
        [Required] public required string Nombre { get; set; }
        [Required] [EmailAddress] public required string Email { get; set; }
        [Required] [MinLength(6)] public required string Password { get; set; }
        public string? Direccion { get; set; }
        public string? Descripcion { get; set; }
    }
}