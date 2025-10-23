using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class LibroCreacionDTO
    {
        [Required]
        [StringLength(150, ErrorMessage = "El campo {0} debe de tener {1} caracteres o menos")]
        public required string Titulo { get; set; }
        public int AutorId { get; set; }
    }
}
