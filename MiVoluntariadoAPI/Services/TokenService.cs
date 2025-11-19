using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiVoluntariadoAPI.Entities;

namespace MiVoluntariadoAPI.Services
{
    public class TokenService : ITokenService
    {
        // Usamos SymmetricSecurityKey para encriptar/firmar el token
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            // Obtenemos la clave secreta desde appsettings.json
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        }

        public string CreateToken(Usuario usuario)
        {
            // 1. Definimos los CLAIMS (Datos que van dentro del token)
            var claims = new List<Claim>
            {
                // Guardamos el ID del usuario (importante para saber quién es luego)
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                // Guardamos el Email
                new Claim(ClaimTypes.Name, usuario.Email),
                // Guardamos el Rol (Voluntario o Admin)
                new Claim(ClaimTypes.Role, usuario.TipoUsuario) 
            };

            // 2. Creamos las credenciales de firma
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 3. Describimos el token (expiración, firmante, etc.)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // El token dura 7 días
                SigningCredentials = creds,
                Issuer = "https://tuapi.com",    // Debe coincidir con appsettings
                Audience = "https://tuappmovil.com" // Debe coincidir con appsettings
            };

            // 4. Generamos el token final
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string CreateToken(Empresa empresa)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, empresa.Id.ToString()),
                new Claim(ClaimTypes.Name, empresa.Email),
                // Para empresas, el rol es fijo: "Empresa"
                new Claim(ClaimTypes.Role, "Empresa") 
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = "https://tuapi.com",
                Audience = "https://tuappmovil.com"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}