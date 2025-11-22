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
    public class CertificadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificadosController(AppDbContext context) => _context = context;

        // =========================================================================
        // 1. POST /api/certificados/generar (GENERAR CERTIFICADO) - CORREGIDO
        // Ahora acepta GenerarCertificadoDto como entrada
        // =========================================================================
        [Authorize(Roles = "Empresa,Admin")]
        [HttpPost("generar")]
        public async Task<ActionResult<CertificadoDto>> GenerarCertificado(GenerarCertificadoDto dto)
        {
            // 1. BUSCAR ACTIVIDAD para obtener el EmpresaId
            var actividad = await _context.Actividades.FindAsync(dto.ActividadId);
            if (actividad == null)
            {
                return NotFound($"Actividad con ID {dto.ActividadId} no encontrada.");
            }
            
            // 2. Opcional: Validar que el usuario (Voluntario) exista
            var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);
            if (usuario == null)
            {
                return NotFound($"Usuario con ID {dto.UsuarioId} no encontrado.");
            }
            
            // 3. CREAR ENTIDAD CERTIFICADO con los IDs obtenidos/enviados
            var certificado = new Certificado
            {
                UsuarioId = dto.UsuarioId,              // Asignación correcta
                ActividadId = dto.ActividadId,          // Asignación correcta
                EmpresaId = actividad.EmpresaId,        // Obtenido de la Actividad
                HorasCertificadas = dto.HorasCertificadas,
                UrlCertificadoPDF = "https://fake-url.com/pdf",
                FechaEmision = DateTime.UtcNow
            };

            _context.Certificados.Add(certificado);
            await _context.SaveChangesAsync();

            // 4. Retornar el DTO de salida con datos completos
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
        // 2. GET /api/certificados/usuario/{id} (CONSULTAR POR USUARIO)
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