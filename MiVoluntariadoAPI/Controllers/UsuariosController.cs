using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiVoluntariadoAPI.Data;
using MiVoluntariadoAPI.DTOs.Core;

namespace MiVoluntariadoAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context) => _context = context;

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var u = await _context.Usuarios.FindAsync(id);
            if (u == null) return NotFound();

            return new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email, // El email se puede ver, pero no es el hash
                CurriculumSocial = u.CurriculumSocial,
                FotoPerfilURL = u.FotoPerfilURL,
                TipoUsuario = u.TipoUsuario
            };
        }

        // PUT: api/usuarios/{id}
        // CORRECCIÓN: Usamos UpdateUsuarioDto para restringir campos sensibles
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUsuario(int id, UpdateUsuarioDto updateDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Solo actualizamos lo permitido
            usuario.Nombre = updateDto.Nombre;
            usuario.Apellido = updateDto.Apellido;
            usuario.CurriculumSocial = updateDto.CurriculumSocial;
            usuario.FotoPerfilURL = updateDto.FotoPerfilURL;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        // DELETE: api/usuarios/{id}
        [Authorize(Roles = "Admin")] // Solo admin puede borrar usuarios
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}