namespace MiVoluntariadoAPI.DTOs.Core
{
    public class PostulacionDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty; // Para que la empresa sepa quién es
        public int ActividadId { get; set; }
        public string NombreActividad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaPostulacion { get; set; }
        public int HorasAsignadas { get; set; }
        public int HorasCompletadas { get; set; }
    }
}