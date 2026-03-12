using GestionDeDevoluciones.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionDeDevoluciones.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext contexto)
        {
            Console.WriteLine("➡️ DbInitializer iniciado.");
            try 
            {
                contexto.Database.EnsureCreated();
                
                contexto.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Observaciones') AND name = 'Descripcion')
                        ALTER TABLE dbo.Observaciones ADD Descripcion nvarchar(500) DEFAULT '' NOT NULL;
                        
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DecisionesAdoptadas') AND name = 'Descripcion')
                        ALTER TABLE dbo.DecisionesAdoptadas ADD Descripcion nvarchar(max) DEFAULT '' NOT NULL;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Usuarios') AND name = 'ContrasenaHash')
                        ALTER TABLE dbo.Usuarios ADD ContrasenaHash nvarchar(max) DEFAULT '' NOT NULL;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Productos') AND name = 'Construccion')
                        ALTER TABLE dbo.Productos ADD Construccion nvarchar(120) NULL;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Productos') AND name = 'Proteccion')
                        ALTER TABLE dbo.Productos ADD Proteccion nvarchar(120) NULL;
                        
                    IF EXISTS (SELECT 1 FROM Roles WHERE RolId = 1) UPDATE Roles SET Nombre = 'Administrador' WHERE RolId = 1;
                    IF EXISTS (SELECT 1 FROM Roles WHERE RolId = 2) UPDATE Roles SET Nombre = 'Operador' WHERE RolId = 2;
                ");

                if (!contexto.Roles.Any())
                {
                    contexto.Roles.AddRange(new List<Rol> { new Rol { Nombre = "Administrador" }, new Rol { Nombre = "Operador" } });
                    contexto.SaveChanges();
                }

                if (!contexto.TiposEstado.Any())
                {
                    contexto.TiposEstado.AddRange(new List<TipoEstado> { 
                        new TipoEstado { Estado = "Pendiente" }, 
                        new TipoEstado { Estado = "En Revisión" }, 
                        new TipoEstado { Estado = "Aceptada" }, 
                        new TipoEstado { Estado = "Rechazada" } 
                    });
                    contexto.SaveChanges();
                }

                if (!contexto.Personal.Any())
                {
                    contexto.Personal.Add(new Personal { Nombre = "Técnico WEG", Legajo = "T-001" });
                    contexto.Personal.Add(new Personal { Nombre = "Admin Facturación", Legajo = "F-001" });
                }

                var areasRequeridas = new[] { "Asistencia Técnica", "Ventas", "Facturación", "Expedición" };
                foreach (var area in areasRequeridas)
                {
                    if (!contexto.Personal.Any(p => p.Nombre == area))
                    {
                        contexto.Personal.Add(new Personal { 
                            Nombre = area, 
                            Legajo = $"AREA-{area.Substring(0, 3).ToUpper()}" 
                        });
                    }
                }
                contexto.SaveChanges();

                if (!contexto.Clientes.Any())
                {
                    var direccion = new Direccion { Calle = "Bv. 25 de Mayo", Numero = "1234", Ciudad = "San Francisco" };
                    contexto.Direcciones.Add(direccion);
                    contexto.SaveChanges();

                    contexto.Clientes.Add(new Cliente { RazonSocial = "WEG Argentina S.A.", CodigoCliente = "CLI-WEG-01", DireccionId = direccion.DireccionId });
                    contexto.SaveChanges();
                }

                if (!contexto.Usuarios.Any())
                {
                    var rolAdmin = contexto.Roles.FirstOrDefault(r => r.Nombre == "Administrador");
                    if (rolAdmin != null) contexto.Usuarios.Add(new Usuario { Nombre = "Administrador", Email = "admin@weg.com", ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("admin123"), RolId = rolAdmin.RolId });
                    contexto.SaveChanges();
                }
                else
                {
                    var administrador = contexto.Usuarios.FirstOrDefault(u => u.Email == "admin@weg.com");
                    if (administrador != null)
                    {
                        administrador.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                        contexto.SaveChanges();
                    }
                }
                Console.WriteLine("✅ Seeding finalizado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR CRÍTICO EN INITIALIZE: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
        }
    }
}
