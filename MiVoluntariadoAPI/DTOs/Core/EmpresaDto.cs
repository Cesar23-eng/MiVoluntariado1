namespace MiVoluntariadoAPI.DTOs.Core
{
    public class EmpresaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
        public string? LogoURL { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}