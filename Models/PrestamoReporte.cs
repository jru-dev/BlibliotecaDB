using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models
{
    /// <summary>
    /// Una fila del reporte: un libro dentro de un préstamo (resultado del INNER JOIN de 4 tablas).
    /// </summary>
    public class PrestamoReporte
    {
        public int PrestamoId { get; set; }
        public int SocioId { get; set; }
        public string Socio { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string Libro { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaPrestamo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaLimite { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", NullDisplayText = "Pendiente")]
        public DateTime? FechaDevolucion { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}
