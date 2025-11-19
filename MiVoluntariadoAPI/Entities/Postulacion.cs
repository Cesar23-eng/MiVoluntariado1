using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiVoluntariadoAPI.Entities
{
    public class Postulacion
    {
        [Key]
        public int Id { get; set; }

        // Clave Foránea Usuario
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        // Clave Foránea Actividad
        [ForeignKey("Actividad")]
        public int ActividadId { get; set; }
        public Actividad? Actividad { get; set; }

        // Ej: "Pendiente", "Aprobada", "Finalizada"
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaPostulacion { get; set; } = DateTime.UtcNow;

        public int HorasAsignadas { get; set; } = 0;
        public int HorasCompletadas { get; set; } = 0;
    }
}