using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Autor")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Disponible es obligatorio.")]
        [Display(Name = "Disponible")]
        public string Disponible { get; set; } = "Sí";

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de publicación")]
        public DateTime FechaDePublicacion { get; set; }

        [Display(Name = "URL de portada")]
        public string? Portada { get; set; }

        [Required(ErrorMessage = "El título del libro es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Título del libro")]
        public string TituloLibro { get; set; } = string.Empty;

        [Display(Name = "Descripción del libro")]
        public string? DescripcionLibro { get; set; }

        [NotMapped]
        [Display(Name = "Portada (archivo)")]
        public IFormFile? ArchivoPortada { get; set; }

    }
}
