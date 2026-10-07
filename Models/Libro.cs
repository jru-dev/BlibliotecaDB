using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models
{
    public class Libro
    {
        public int LibroId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "El título debe tener entre 2 y 150 caracteres.")]
        [DataType(DataType.Text)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ISBN es obligatorio.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "El ISBN debe tener entre 10 y 20 caracteres.")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; } = string.Empty;

        // El valor 0 corresponde a la opción "-- Seleccione --" de la lista desplegable.
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un autor.")]
        [Display(Name = "Autor")]
        public int AutorId { get; set; }

        [Required(ErrorMessage = "Los ejemplares son obligatorios.")]
        [Range(0, 1000, ErrorMessage = "Los ejemplares deben estar entre 0 y 1000.")]
        [Display(Name = "Ejemplares")]
        public int Ejemplares { get; set; }

        public bool Activo { get; set; } = true;

        // Viene del INNER JOIN con Autores (alias NombreAutor en los procedimientos).
        [Display(Name = "Autor")]
        public string? NombreAutor { get; set; }
    }
}
