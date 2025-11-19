namespace MiVoluntariadoAPI.DTOs.Core
{
    public class UpdateEmpresaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
        public string? LogoURL { get; set; }
    }
}