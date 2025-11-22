using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // ... (El resto de los métodos GET/PUT/DELETE de Usuario)

        // GET: api/usuarios/{id}/certificados - (getCertificadosByUsuario)
        [HttpGet("{id}/certificados")]
        public async Task<ActionResult<IEnumerable<CertificadoDto>>> GetCertificados(int id)
        {
            var certificados = await _context.Certificados
                .Include(c => c.Actividad)
                .Include(c => c.Empresa)
                .Include(c => c.Usuario)
                .Where(c => c.UsuarioId == id)
                .Select(c => new CertificadoDto
                {
                    Id = c.Id,
                    NombreVoluntario = c.Usuario!.Nombre + " " + c.Usuario.Apellido,
                    NombreEmpresa = c.Empresa!.Nombre,
                    NombreActividad = c.Actividad!.NombreActividad,
                    HorasCertificadas = c.HorasCertificadas,
                    FechaEmision = c.FechaEmision,
                    UrlCertificadoPDF = c.UrlCertificadoPDF
                })
                .ToListAsync();

            return Ok(certificados);
        }

        // GET: api/usuarios/{id}/postulaciones - (getPostulaciones) para el usuario
        [HttpGet("{id}/postulaciones")]
        public async Task<ActionResult<IEnumerable<PostulacionDto>>> GetPostulaciones(int id)
        {
            var postulaciones = await _context.Postulaciones
                .Include(p => p.Actividad)
                .Where(p => p.UsuarioId == id)
                .Select(p => new PostulacionDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
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