using System.ComponentModel.DataAnnotations;

namespace MiVoluntariadoAPI.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public required string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        // "Voluntario" o "Admin"
        [Required]
        public required string TipoUsuario { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public string? FotoPerfilURL { get; set; }

        public string? CurriculumSocial { get; set; }

        // Propiedades de Navegación (Relaciones)
        public ICollection<Postulacion>? Postulaciones { get; set; }
        public ICollection<Certificado>? Certificados { get; set; }
    }
}