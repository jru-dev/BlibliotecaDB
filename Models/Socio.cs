using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models
{
    public class Socio
    {
        public int SocioId { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El DNI debe tener entre 8 y 15 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El DNI solo debe contener números.")]
        [Display(Name = "DNI")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; }

        public bool Activo { get; set; } = true;
    }
}
