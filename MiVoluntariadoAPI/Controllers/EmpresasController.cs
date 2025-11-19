using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiVoluntariadoAPI.Data;
using MiVoluntariadoAPI.DTOs.Core;

namespace MiVoluntariadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresasController(AppDbContext context) => _context = context;

        // GET: api/empresas
        // CORRECCIÓN: Devolvemos DTOs para no mostrar PasswordHash
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetEmpresas()
        {
            return await _context.Empresas
                .Select(e => new EmpresaDto 
                { 
                    Id = e.Id, 
                    Nombre = e.Nombre, 
                    Descripcion = e.Descripcion,
                    Direccion = e.Direccion,
                    LogoURL = e.LogoURL,
                    Email = e.Email 
                    // NO enviamos token ni password
                })
                .ToListAsync();
        }

        // GET: api/empresas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpresaDto>> GetEmpresa(int id)
        {
            var e = await _context.Empresas.FindAsync(id);
            if (e == null) return NotFound();

            return new EmpresaDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                Direccion = e.Direccion,
                LogoURL = e.LogoURL,
                Email = e.Email
            };
        }

        // PUT: api/empresas/{id}
        // Solo Admin o la propia empresa deberían poder editar
        [Authorize(Roles = "Admin,Empresa")] 
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmpresa(int id, UpdateEmpresaDto updateDto)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound();

            empresa.Nombre = updateDto.Nombre;
            empresa.Descripcion = updateDto.Descripcion;
            empresa.Direccion = updateDto.Direccion;
            empresa.LogoURL = updateDto.LogoURL;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/empresas/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmpresa(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound();

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}