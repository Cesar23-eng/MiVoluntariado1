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

        [Authorize(Roles = "Empresa,Admin")]
        [HttpPost("generar")]
        public async Task<ActionResult<CertificadoDto>> GenerarCertificado(CertificadoDto dto)
        {
            // 1. Crear la entidad
            var certificado = new Certificado
            {
                // OJO: Asumimos que el frontend manda los IDs en el DTO aunque no los mostré antes. 
                // Si el DTO no tiene los IDs, agrégalos a CertificadoDto (UsuarioId, EmpresaId, ActividadId)
                // Aquí uso valores fijos SOLO si tu DTO no los tiene, pero deberías agregarlos al DTO de entrada.
                UsuarioId = 1, // <--- DEBES PASAR ESTO DESDE EL FRONT
                EmpresaId = 1, // <--- DEBES PASAR ESTO DESDE EL FRONT
                ActividadId = 1, // <--- DEBES PASAR ESTO DESDE EL FRONT
                HorasCertificadas = dto.HorasCertificadas,
                UrlCertificadoPDF = "https://fake-url.com/pdf",
                FechaEmision = DateTime.UtcNow
            };

            _context.Certificados.Add(certificado);
            await _context.SaveChangesAsync();

            // 2. CORRECCIÓN: Cargar las relaciones para devolver los NOMBRES y no NULL
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
        
        // ... (El método GET sigue igual, ya incluía los Includes)
    }
}