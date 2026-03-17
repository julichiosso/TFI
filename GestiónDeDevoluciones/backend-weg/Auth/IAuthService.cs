using GestionDeDevoluciones.Backend.DTOs;

namespace GestionDeDevoluciones.Backend.Auth
{
    public interface IAuthService
    {
        string Registrar(RegisterDTO dto);
        string IniciarSesion(LoginDTO dto);
    }
}
