using MiVoluntariadoAPI.Entities;

namespace MiVoluntariadoAPI.Services
{
    public interface ITokenService
    {
        // Un método para usuarios (Voluntarios y Admins)
        string CreateToken(Usuario usuario);

        // Un método específico para Empresas
        string CreateToken(Empresa empresa);
    }
}