using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiVoluntariadoAPI.Data;
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

        // POST: api/actividades/{id}/postular
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

        // PUT: api/postulaciones/{id}/aprobar
        [Authorize(Roles = "Empresa")]
        [HttpPut("{id}/aprobar")]
        public async Task<ActionResult> AprobarPostulacion(int id)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null) return NotFound();

            postulacion.Estado = "Aprobada";
            await _context.SaveChangesAsync();

            return Ok("Postulación aprobada.");
        }
        
        // NUEVO ENDPOINT: PUT: api/postulaciones/{id}/rechazar
        [Authorize(Roles = "Empresa")]
        [HttpPut("{id}/rechazar")]
        public async Task<ActionResult> RechazarPostulacion(int id)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null) return NotFound();

            postulacion.Estado = "Rechazada"; // Cambiamos el estado
            await _context.SaveChangesAsync();

            return Ok("Postulación rechazada.");
        }

        // PUT: api/postulaciones/{id}/finalizar
        [Authorize(Roles = "Empresa")]
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
    }
}