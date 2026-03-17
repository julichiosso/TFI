using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestionDeDevoluciones.Backend.DTOs
{
    public class LoginDTO
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("password")]
        public string Contrasena { get; set; } = string.Empty;
    }
}
