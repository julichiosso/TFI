using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionDeDevoluciones.Backend.DTOs;

namespace GestionDeDevoluciones.Backend.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _contexto;
        private readonly IConfiguration _configuracion;

        public AuthService(AppDbContext contexto, IConfiguration configuracion)
        {
            _contexto = contexto;
            _configuracion = configuracion;
        }

        public string Registrar(RegisterDTO dto)
        {
            if (_contexto.Usuarios.Any(u => u.Email == dto.Email))
                throw new Exception("El email ya está registrado.");

            var usuario = new Usuario
            {
                Nombre = dto.NombreUsuario,
                Email = dto.Email,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                RolId = dto.RolId
            };

            _contexto.Usuarios.Add(usuario);
            _contexto.SaveChanges();

            return GenerarTokenJwt(usuario);
        }

        public string IniciarSesion(LoginDTO dto)
        {
            var usuario = _contexto.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == dto.Email);

            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash))
                throw new Exception("Contraseña incorrecta.");

            return GenerarTokenJwt(usuario);
        }

        private string GenerarTokenJwt(Usuario usuario)
        {
            var reclamos = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Operador"),
            };

            var clave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuracion["Jwt:Key"] ?? "superSecretKey1234567890123456")
            );

            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: reclamos,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
