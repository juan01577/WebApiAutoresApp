using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }

        [Required]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set;}
        public List<Comentario> Comentarios { get; set;}
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
