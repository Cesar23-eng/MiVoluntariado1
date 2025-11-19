using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiVoluntariadoAPI.Data;
using MiVoluntariadoAPI.DTOs.Core;
using MiVoluntariadoAPI.Entities;

namespace MiVoluntariadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActividadesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ActividadesController(AppDbContext context) => _context = context;

        [Authorize(Roles = "Empresa")]
        [HttpPost("/api/empresas/{id}/actividades")]
        public async Task<ActionResult<ActividadDto>> CreateActividad(int id, ActividadDto actividadDto)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound("Empresa no encontrada");

            var actividad = new Actividad
            {
                EmpresaId = id,
                NombreActividad = actividadDto.NombreActividad,
                Descripcion = actividadDto.Descripcion,
                FechaInicio = actividadDto.FechaInicio,
                FechaFin = actividadDto.FechaFin,
                Cupos = actividadDto.Cupos,
                Estado = true // CORRECCIÓN: Booleano, true al crear
            };

            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();

            actividadDto.Id = actividad.Id;
            actividadDto.EmpresaId = id;
            actividadDto.NombreEmpresa = empresa.Nombre;
            actividadDto.Estado = "Disponible"; // Mapeo visual para el DTO

            return Ok(actividadDto);
        }

        // PUT: api/actividades/{id}
        [Authorize(Roles = "Empresa")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateActividad(int id, UpdateActividadDto dto)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null) return NotFound();

            actividad.NombreActividad = dto.NombreActividad;
            actividad.Descripcion = dto.Descripcion;
            actividad.FechaInicio = dto.FechaInicio;
            actividad.FechaFin = dto.FechaFin;
            actividad.Cupos = dto.Cupos;
            actividad.Estado = dto.Estado; // Se actualiza directo el bool

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/actividades/{id}
        [Authorize(Roles = "Empresa,Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActividad(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null) return NotFound();

            _context.Actividades.Remove(actividad);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActividadDto>>> GetActividades()
        {
            return await _context.Actividades
                .Include(a => a.Empresa)
                .Select(a => new ActividadDto
                {
                    Id = a.Id,
                    EmpresaId = a.EmpresaId,
                    NombreEmpresa = a.Empresa!.Nombre,
                    NombreActividad = a.NombreActividad,
                    Descripcion = a.Descripcion,
                    FechaInicio = a.FechaInicio,
                    FechaFin = a.FechaFin,
                    Cupos = a.Cupos,
                    // Convertimos el bool de BD a string para el DTO visual
                    Estado = a.Estado ? "Disponible" : "Cerrada"
                })
                .ToListAsync();
        }
    }
}