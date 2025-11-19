namespace MiVoluntariadoAPI.DTOs.Core
{
    public class CertificadoDto
    {
        public int Id { get; set; }
        public string NombreVoluntario { get; set; } = string.Empty;
        public string NombreEmpresa { get; set; } = string.Empty;
        public string NombreActividad { get; set; } = string.Empty;
        public int HorasCertificadas { get; set; }
        public DateTime FechaEmision { get; set; }
        public string UrlCertificadoPDF { get; set; } = string.Empty;
    }
}