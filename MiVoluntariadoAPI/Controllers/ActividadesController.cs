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

        // 1. POST /api/empresas/{id}/actividades (CREAR)
        [Authorize(Roles = "Empresa,Admin")]
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
                Estado = true // Booleano: se crea como Disponible
            };

            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();

            actividadDto.Id = actividad.Id;
            actividadDto.EmpresaId = id;
            actividadDto.NombreEmpresa = empresa.Nombre;
            actividadDto.Estado = "Disponible";

            return Ok(actividadDto);
        }

        // 2. GET /api/actividades (LISTAR TODAS)
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
                    Estado = a.Estado ? "Disponible" : "Cerrada"
                })
                .ToListAsync();
        }

        // 3. GET /api/actividades/{id} (VER DETALLE - getActividad)
        [HttpGet("{id}")]
        public async Task<ActionResult<ActividadDto>> GetActividad(int id)
        {
            var actividad = await _context.Actividades
                .Include(a => a.Empresa)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actividad == null) return NotFound();

            return new ActividadDto
            {
                Id = actividad.Id,
                EmpresaId = actividad.EmpresaId,
                NombreEmpresa = actividad.Empresa!.Nombre,
                NombreActividad = actividad.NombreActividad,
                Descripcion = actividad.Descripcion,
                FechaInicio = actividad.FechaInicio,
                FechaFin = actividad.FechaFin,
                Cupos = actividad.Cupos,
                Estado = actividad.Estado ? "Disponible" : "Cerrada"
            };
        }

        // 4. PUT /api/actividades/{id} (ACTUALIZAR)
        [Authorize(Roles = "Empresa,Admin")]
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
            actividad.Estado = dto.Estado; 

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. DELETE /api/actividades/{id} (ELIMINAR)
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
        
        // 6. GET /api/actividades/{id}/postulaciones (VER POSTULACIONES - getPostulaciones para Empresas)
        [Authorize(Roles = "Empresa,Admin")]
        [HttpGet("{id}/postulaciones")]
        public async Task<ActionResult<IEnumerable<PostulacionDto>>> GetPostulacionesPorActividad(int id)
        {
            var postulaciones = await _context.Postulaciones
                .Include(p => p.Usuario)
                .Where(p => p.ActividadId == id)
                .Select(p => new PostulacionDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    NombreUsuario = p.Usuario!.Nombre + " " + p.Usuario.Apellido,
                    ActividadId = p.ActividadId,
                    Estado = p.Estado,
                    FechaPostulacion = p.FechaPostulacion,
                    HorasAsignadas = p.HorasAsignadas,
                    HorasCompletadas = p.HorasCompletadas
                })
                .ToListAsync();
            
            if (!postulaciones.Any()) return NotFound("No hay postulaciones para esta actividad.");

            return Ok(postulaciones);
        }
    }
}