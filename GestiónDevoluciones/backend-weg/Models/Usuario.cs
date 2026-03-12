using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestionDeDevoluciones.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }
 
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres")]
        public string Nombre { get; set; } = string.Empty;
 
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(80, ErrorMessage = "El correo no puede superar los 80 caracteres")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
 
        [JsonPropertyName("passwordHash")]
        public string ContrasenaHash { get; set; } = string.Empty;
 
        [Required(ErrorMessage = "El rol es obligatorio")]
        public int RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        public Rol? Rol { get; set; }
    }
}
