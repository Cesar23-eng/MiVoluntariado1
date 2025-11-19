using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiVoluntariadoAPI.Entities
{
    public class Actividad
    {
        [Key]
        public int Id { get; set; }

        // Clave Foránea
        [ForeignKey("Empresa")]
        public int EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }

        [Required]
        [StringLength(200)]
        public required string NombreActividad { get; set; }

        public string? Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int Cupos { get; set; }

        public bool Estado { get; set; } = true; // true = Disponible, false = Cerrada

        // Propiedades de Navegación
        public ICollection<Postulacion>? Postulaciones { get; set; }
    }
}