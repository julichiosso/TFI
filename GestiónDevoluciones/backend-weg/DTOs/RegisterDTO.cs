using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestionDeDevoluciones.Backend.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [JsonPropertyName("password")]
        public string Contrasena { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }
    }
}
