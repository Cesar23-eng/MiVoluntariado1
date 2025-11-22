using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MiVoluntariadoAPI.DTOs.Core
{
    // DTO utilizado SÓLO para recibir datos al generar un certificado
    public class GenerarCertificadoDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de Usuario es requerido.")]
        public int UsuarioId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de Actividad es requerido.")]
        public int ActividadId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Las horas certificadas deben ser al menos 1.")]
        public int HorasCertificadas { get; set; }
    }
}