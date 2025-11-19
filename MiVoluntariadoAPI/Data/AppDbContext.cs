using Microsoft.EntityFrameworkCore;
using MiVoluntariadoAPI.Entities;

namespace MiVoluntariadoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablas de la Base de Datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<Postulacion> Postulaciones { get; set; }
        public DbSet<Certificado> Certificados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuración de Relaciones para evitar errores de SQL Server ---
            // Usamos .OnDelete(DeleteBehavior.Restrict) para evitar que al borrar
            // un Usuario o Empresa se borren automáticamente los historiales
            // de forma conflictiva.

            // Configuración Postulacion
            modelBuilder.Entity<Postulacion>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Postulaciones)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Postulacion>()
                .HasOne(p => p.Actividad)
                .WithMany(a => a.Postulaciones)
                .HasForeignKey(p => p.ActividadId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración Certificado
            modelBuilder.Entity<Certificado>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Certificados)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificado>()
                .HasOne(c => c.Empresa)
                .WithMany(e => e.Certificados)
                .HasForeignKey(c => c.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificado>()
                .HasOne(c => c.Actividad)
                .WithMany() // Una actividad puede tener muchos certificados emitidos, pero no agregamos la lista en Actividad para no ensuciarla
                .HasForeignKey(c => c.ActividadId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}