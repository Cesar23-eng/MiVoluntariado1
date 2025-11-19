using System.ComponentModel.DataAnnotations;

namespace MiVoluntariadoAPI.DTOs.Core
{
    public class UpdateUsuarioDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Apellido { get; set; } = string.Empty;
        public string? CurriculumSocial { get; set; }
        public string? FotoPerfilURL { get; set; }
    }
}