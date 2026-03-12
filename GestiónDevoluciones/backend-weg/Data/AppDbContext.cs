using GestionDeDevoluciones.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionDeDevoluciones.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opciones)
            : base(opciones) { }

        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoEstado> TiposEstado { get; set; }
        public DbSet<Remito> Remitos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Personal> Personal { get; set; }
        public DbSet<Observaciones> Observaciones { get; set; }
        public DbSet<DecisionAdoptada> DecisionesAdoptadas { get; set; }

        protected override void OnModelCreating(ModelBuilder constructorModelos)
        {
            base.OnModelCreating(constructorModelos);

            constructorModelos.Entity<Cliente>()
                .HasOne(c => c.Direccion)
                .WithOne()
                .HasForeignKey<Cliente>(c => c.DireccionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            constructorModelos.Entity<Remito>()
                .HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            constructorModelos.Entity<Remito>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            constructorModelos.Entity<Remito>()
                .HasOne(r => r.TipoEstado)
                .WithMany()
                .HasForeignKey(r => r.TipoEstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}