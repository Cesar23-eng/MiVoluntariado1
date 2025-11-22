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
    public class CertificadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificadosController(AppDbContext context) => _context = context;

        // =========================================================================
        // 1. POST /api/certificados/generar (GENERAR CERTIFICADO)
        //
        // =========================================================================
        [Authorize(Roles = "Empresa,Admin")]
        [HttpPost("generar")]
        public async Task<ActionResult<CertificadoDto>> GenerarCertificado(CertificadoDto dto)
        {
            // Nota: En una implementación real, aquí se verificaría que el UsuarioId, 
            // EmpresaId y ActividadId vengan correctamente en el DTO o se extraigan de la lógica de negocio.

            var certificado = new Certificado
            {
                UsuarioId = 1, // <--- REEMPLAZAR con datos reales
                EmpresaId = 1, // <--- REEMPLAZAR con datos reales
                ActividadId = 1, // <--- REEMPLAZAR con datos reales
                HorasCertificadas = dto.HorasCertificadas,
                UrlCertificadoPDF = "https://fake-url.com/pdf",
                FechaEmision = DateTime.UtcNow
            };

            _context.Certificados.Add(certificado);
            await _context.SaveChangesAsync();

            // Retorna el DTO con información completa de las entidades relacionadas
            var certificadoCompleto = await _context.Certificados
                .Include(c => c.Usuario)
                .Include(c => c.Empresa)
                .Include(c => c.Actividad)
                .FirstOrDefaultAsync(c => c.Id == certificado.Id);

            return Ok(new CertificadoDto
            {
                Id = certificadoCompleto!.Id,
                NombreVoluntario = $"{certificadoCompleto.Usuario!.Nombre} {certificadoCompleto.Usuario.Apellido}",
                NombreEmpresa = certificadoCompleto.Empresa!.Nombre,
                NombreActividad = certificadoCompleto.Actividad!.NombreActividad,
                HorasCertificadas = certificadoCompleto.HorasCertificadas,
                FechaEmision = certificadoCompleto.FechaEmision,
                UrlCertificadoPDF = certificadoCompleto.UrlCertificadoPDF
            });
        }

        // =========================================================================
        // 2. GET /api/certificados/usuario/{id} (CONSULTAR POR USUARIO - getCertificadosByUsuario)
        //
        // =========================================================================
        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<IEnumerable<CertificadoDto>>> GetCertificadosUsuario(int id)
        {
            var certs = await _context.Certificados
                .Include(c => c.Empresa)
                .Include(c => c.Actividad)
                .Include(c => c.Usuario)
                .Where(c => c.UsuarioId == id)
                .Select(c => new CertificadoDto
                {
                    Id = c.Id,
                    NombreVoluntario = c.Usuario!.Nombre + " " + c.Usuario.Apellido,
                    NombreEmpresa = c.Empresa!.Nombre,
                    NombreActividad = c.Actividad!.NombreActividad,
                    HorasCertificadas = c.HorasCertificadas,
                    UrlCertificadoPDF = c.UrlCertificadoPDF,
                    FechaEmision = c.FechaEmision
                })
                .ToListAsync();

            if (!certs.Any()) return NotFound("No se encontraron certificados para este usuario.");

            return Ok(certs);
        }

        // =========================================================================
        // 3. GET /api/certificados (LISTAR TODOS - Solo Admin)
        //
        // =========================================================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificadoDto>>> GetAllCertificados()
        {
            var certificados = await _context.Certificados
                .Include(c => c.Empresa)
                .Include(c => c.Actividad)
                .Include(c => c.Usuario)
                .Select(c => new CertificadoDto
                {
                    Id = c.Id,
                    NombreVoluntario = c.Usuario!.Nombre + " " + c.Usuario.Apellido,
                    NombreEmpresa = c.Empresa!.Nombre,
                    NombreActividad = c.Actividad!.NombreActividad,
                    HorasCertificadas = c.HorasCertificadas,
                    UrlCertificadoPDF = c.UrlCertificadoPDF,
                    FechaEmision = c.FechaEmision
                })
                .ToListAsync();

            return Ok(certificados);
        }
    }
}