using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiVoluntariadoAPI.Entities
{
    public class Certificado
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [ForeignKey("Empresa")]
        public int EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }

        [ForeignKey("Actividad")]
        public int ActividadId { get; set; }
        public Actividad? Actividad { get; set; }

        public int HorasCertificadas { get; set; }

        public DateTime FechaEmision { get; set; } = DateTime.UtcNow;

        [Required]
        public required string UrlCertificadoPDF { get; set; }
    }
}