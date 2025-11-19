using System.ComponentModel.DataAnnotations;

namespace MiVoluntariadoAPI.Entities
{
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public required string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public string? Direccion { get; set; }

        public string? LogoURL { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        // Propiedades de Navegación (Relaciones)
        public ICollection<Actividad>? Actividades { get; set; }
        public ICollection<Certificado>? Certificados { get; set; }
    }
}