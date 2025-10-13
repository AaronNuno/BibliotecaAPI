using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The field {0} is require")]
        [StringLength(150,ErrorMessage = "The field {0} must not be longer than {1} characters or less")]
        public required string Nombre { get; set; }
        public List<Libro> Libros { get; set; } = new List<Libro>();

        /*[Range(18, 120)]
        public int Edad {  get; set; }
        [CreditCard]
        public string? TarjetaCredito { get; set; }
         [Url]
        public string? URL   { get; set; }
       */
    }
}
