using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiVoluntariadoAPI.Data;
using MiVoluntariadoAPI.DTOs.Auth;
using MiVoluntariadoAPI.DTOs.Core;
using MiVoluntariadoAPI.Entities;
using MiVoluntariadoAPI.Services;
using System.Security.Cryptography;
using System.Text;

namespace MiVoluntariadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST: api/auth/register/voluntario
        [HttpPost("register/voluntario")]
        public async Task<ActionResult<UsuarioDto>> RegisterVoluntario(RegisterVoluntarioDto registerDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email.ToLower()))
                return BadRequest("El email ya está en uso.");

            using var hmac = new HMACSHA512();

            var usuario = new Usuario
            {
                Nombre = registerDto.Nombre,
                Apellido = registerDto.Apellido,
                Email = registerDto.Email.ToLower(),
                TipoUsuario = "Voluntario",
                // Simulamos Salt+Hash guardándolo en un solo string separado por punto
                PasswordHash = Convert.ToBase64String(hmac.Key) + "." + Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)))
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                Token = _tokenService.CreateToken(usuario)
            };
        }

        // POST: api/auth/register/empresa
        [HttpPost("register/empresa")]
        public async Task<ActionResult<EmpresaDto>> RegisterEmpresa(RegisterEmpresaDto registerDto)
        {
            if (await _context.Empresas.AnyAsync(e => e.Email == registerDto.Email.ToLower()))
                return BadRequest("El email ya está en uso.");

            using var hmac = new HMACSHA512();

            var empresa = new Empresa
            {
                Nombre = registerDto.Nombre,
                Email = registerDto.Email.ToLower(),
                Direccion = registerDto.Direccion,
                Descripcion = registerDto.Descripcion,
                PasswordHash = Convert.ToBase64String(hmac.Key) + "." + Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)))
            };

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            return new EmpresaDto
            {
                Id = empresa.Id,
                Nombre = empresa.Nombre,
                Email = empresa.Email,
                Token = _tokenService.CreateToken(empresa)
            };
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDto loginDto)
        {
            // 1. Intentar buscar como Usuario (Voluntario/Admin)
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == loginDto.Email.ToLower());
            if (usuario != null)
            {
                if (!ValidarPassword(loginDto.Password, usuario.PasswordHash)) return Unauthorized("Contraseña incorrecta.");
                
                return new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    TipoUsuario = usuario.TipoUsuario,
                    Token = _tokenService.CreateToken(usuario)
                };
            }

            // 2. Intentar buscar como Empresa
            var empresa = await _context.Empresas.SingleOrDefaultAsync(e => e.Email == loginDto.Email.ToLower());
            if (empresa != null)
            {
                if (!ValidarPassword(loginDto.Password, empresa.PasswordHash)) return Unauthorized("Contraseña incorrecta.");

                return new EmpresaDto
                {
                    Id = empresa.Id,
                    Nombre = empresa.Nombre,
                    Email = empresa.Email,
                    Token = _tokenService.CreateToken(empresa)
                };
            }

            return Unauthorized("Usuario no encontrado.");
        }

        // Método auxiliar para validar el hash personalizado
        private bool ValidarPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(hash);
        }
    }
}