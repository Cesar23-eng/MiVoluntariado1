using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiVoluntariadoAPI.Data;
using MiVoluntariadoAPI.DTOs.Core;
using MiVoluntariadoAPI.Entities;
using System.Security.Claims;

namespace MiVoluntariadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostulacionesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/actividades/{id}/postular (Solo Voluntario - No se recomienda Admin aquí)
        [Authorize(Roles = "Voluntario")]
        [HttpPost("/api/actividades/{id}/postular")]
        public async Task<ActionResult> Postular(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var usuarioId = int.Parse(userIdClaim.Value);

            // Verificar si ya existe postulación
            if (await _context.Postulaciones.AnyAsync(p => p.ActividadId == id && p.UsuarioId == usuarioId))
                return BadRequest("Ya te has postulado a esta actividad.");

            var postulacion = new Postulacion
            {
                UsuarioId = usuarioId,
                ActividadId = id,
                Estado = "Pendiente",
                FechaPostulacion = DateTime.UtcNow
            };

            _context.Postulaciones.Add(postulacion);
            await _context.SaveChangesAsync();

            return Ok("Postulación realizada con éxito.");
        }

        // PUT: api/postulaciones/{id}/aprobar - Permitido a Empresa y Admin
        [Authorize(Roles = "Empresa,Admin")]
        [HttpPut("{id}/aprobar")]
        public async Task<ActionResult> AprobarPostulacion(int id)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null) return NotFound();

            postulacion.Estado = "Aprobada";
            await _context.SaveChangesAsync();

            return Ok("Postulación aprobada.");
        }
        
        // PUT: api/postulaciones/{id}/rechazar - Permitido a Empresa y Admin
        [Authorize(Roles = "Empresa,Admin")]
        [HttpPut("{id}/rechazar")]
        public async Task<ActionResult> RechazarPostulacion(int id)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null) return NotFound();

            postulacion.Estado = "Rechazada"; // Cambiamos el estado
            await _context.SaveChangesAsync();

            return Ok("Postulación rechazada.");
        }

        // PUT: api/postulaciones/{id}/finalizar - Permitido a Empresa y Admin
        [Authorize(Roles = "Empresa,Admin")]
        [HttpPut("{id}/finalizar")]
        public async Task<ActionResult> FinalizarPostulacion(int id, [FromBody] int horasCompletadas)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null) return NotFound();

            postulacion.Estado = "Finalizada";
            postulacion.HorasCompletadas = horasCompletadas;

            await _context.SaveChangesAsync();

            return Ok("Postulación finalizada y horas registradas.");
        }
        
        // NUEVO ENDPOINT: GET: api/postulaciones - Listar todas las postulaciones (Solo Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostulacionDto>>> GetAllPostulaciones()
        {
            var postulaciones = await _context.Postulaciones
                .Include(p => p.Usuario)
                .Include(p => p.Actividad)
                .Select(p => new PostulacionDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    NombreUsuario = p.Usuario!.Nombre + " " + p.Usuario.Apellido,
                    ActividadId = p.ActividadId,
                    NombreActividad = p.Actividad!.NombreActividad,
                    Estado = p.Estado,
                    FechaPostulacion = p.FechaPostulacion,
                    HorasAsignadas = p.HorasAsignadas,
                    HorasCompletadas = p.HorasCompletadas
                })
                .ToListAsync();

            return Ok(postulaciones);
        }
    }
}