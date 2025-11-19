using System.ComponentModel.DataAnnotations;

namespace MiVoluntariadoAPI.DTOs.Core
{
    public class ActividadDto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty; // Útil para el frontend

        [Required] public string NombreActividad { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        [Required] public DateTime FechaInicio { get; set; }
        [Required] public DateTime FechaFin { get; set; }
        [Required] public int Cupos { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}