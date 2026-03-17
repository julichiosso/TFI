using GestionDeDevoluciones.Backend.Auth;
using GestionDeDevoluciones.Services;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddCors(opciones =>
{
    opciones.AddPolicy("AllowFrontend",
        politica => politica.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

constructor.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlServer(
        constructor.Configuration.GetConnectionString("DefaultConnection")
    )
);

constructor.Services.AddScoped<IClienteService, ClienteService>();
constructor.Services.AddScoped<IProductoService, ProductoService>();
constructor.Services.AddScoped<IRemitoService, RemitoService>();
constructor.Services.AddScoped<IPersonalService, PersonalService>();
constructor.Services.AddScoped<IUsuarioService, UsuarioService>();
constructor.Services.AddScoped<IRolService, RolService>();
constructor.Services.AddScoped<IDecisionAdoptadaService, DecisionAdoptadaService>();
constructor.Services.AddScoped<ITipoEstadoService, TipoEstadoService>();
constructor.Services.AddScoped<IObservacionService, ObservacionService>();
constructor.Services.AddScoped<IAuthService, AuthService>();

constructor.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(constructor.Configuration["Jwt:Key"] ?? "superSecretKey1234567890123456")
            )
        };
    });

constructor.Services.AddAuthorization();

constructor.Services.AddControllers()
    .AddJsonOptions(opciones => {
        opciones.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opciones.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
constructor.Services.AddEndpointsApiExplorer();
constructor.Services.AddSwaggerGen();

var aplicacion = constructor.Build();

if (aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseSwagger();
    aplicacion.UseSwaggerUI();
}

aplicacion.UseCors("AllowFrontend");

aplicacion.UseAuthentication();
aplicacion.UseAuthorization();

try 
{
    using (var alcance = aplicacion.Services.CreateScope())
    {
        var servicios = alcance.ServiceProvider;
        var contexto = servicios.GetRequiredService<AppDbContext>();
        Console.WriteLine("🌱 Iniciando DbInitializer...");
        DbInitializer.Initialize(contexto);
        Console.WriteLine("✅ DbInitializer completado.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR EN SEEDING: {ex.Message}");
}

var rutaSubidas = Path.Combine(aplicacion.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(rutaSubidas)) Directory.CreateDirectory(rutaSubidas);
aplicacion.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(rutaSubidas),
    RequestPath = "/uploads"
});

aplicacion.MapControllers();

Console.WriteLine("🚀 Iniciando aplicación en http://localhost:5299 ...");
aplicacion.Run();
