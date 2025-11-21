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

        // POST: api/certificados/generar - Permitido a Empresa y Admin (Ya estaba correcto)
        [Authorize(Roles = "Empresa,Admin")]
        [HttpPost("generar")]
        public async Task<ActionResult<CertificadoDto>> GenerarCertificado(CertificadoDto dto)
        {
            // ... (Lógica de creación de certificado sigue igual)
            var certificado = new Certificado
            {
                UsuarioId = 1, // <--- DEBES PASAR ESTO DESDE EL FRONT
                EmpresaId = 1, // <--- DEBES PASAR ESTO DESDE EL FRONT
                ActividadId = 1, // <--- DEBES PASAR ESTO DESDE EL FRONT
                HorasCertificadas = dto.HorasCertificadas,
                UrlCertificadoPDF = "https://fake-url.com/pdf",
                FechaEmision = DateTime.UtcNow
            };

            _context.Certificados.Add(certificado);
            await _context.SaveChangesAsync();

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

        // GET: api/certificados/usuario/{id} - Permite a cualquier usuario consultar sus propios certificados
        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<IEnumerable<CertificadoDto>>> GetCertificadosUsuario(int id)
        {
            // ... (Lógica de consulta sigue igual)
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

            return Ok(certs);
        }
        
        // NUEVO ENDPOINT: GET: api/certificados - Listar todos los certificados (Solo Admin)
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