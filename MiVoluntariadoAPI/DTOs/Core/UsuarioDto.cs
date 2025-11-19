namespace MiVoluntariadoAPI.DTOs.Core
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FotoPerfilURL { get; set; }
        public string? CurriculumSocial { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        // Token JWT que se devuelve al hacer login
        public string? Token { get; set; } 
    }
}