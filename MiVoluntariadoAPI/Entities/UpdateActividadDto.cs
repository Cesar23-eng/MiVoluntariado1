namespace MiVoluntariadoAPI.DTOs.Core
{
    public class UpdateActividadDto
    {
        public string NombreActividad { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Cupos { get; set; }
        public bool Estado { get; set; } // true/false
    }
}