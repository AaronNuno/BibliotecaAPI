
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The field {0} is require")]
        [StringLength(150,ErrorMessage = "The field {0} must not be longer than {1} characters or less")]
 
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public  string? Identificacion { get; set; }
        public List<AutorLibro> Libros { get; set; } = [];
        [Unicode(false)]
        public string? Foto { get; set; }

    }
}
