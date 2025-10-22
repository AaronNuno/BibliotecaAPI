using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "The field {0} is require")]
        [StringLength(150, ErrorMessage = "The field {0} must not be longer than {1} characters or less")]

        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public string? Identificacion { get; set; }
    }
}
