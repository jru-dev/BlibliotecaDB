using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly IPrestamoRepositorio _prestamos;

        public PrestamosController(IPrestamoRepositorio prestamos)
        {
            _prestamos = prestamos;
        }

        // GET: /Prestamos/Reporte?desde=2026-08-01&hasta=2026-09-30
        public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta)
        {
            // Primera visita (sin filtro): se muestran los últimos 3 meses.
            DateTime fechaDesde = (desde ?? DateTime.Today.AddMonths(-3)).Date;
            DateTime fechaHasta = (hasta ?? DateTime.Today).Date;

            // El rango elegido se conserva en ViewData para rellenar los campos del filtro.
            ViewData["Desde"] = fechaDesde.ToString("yyyy-MM-dd");
            ViewData["Hasta"] = fechaHasta.ToString("yyyy-MM-dd");

            if (fechaDesde > fechaHasta)
            {
                ModelState.AddModelError(string.Empty, "La fecha \"desde\" no puede ser mayor que la fecha \"hasta\".");
                return View(Enumerable.Empty<PrestamoReporte>());
            }

            var filas = await _prestamos.ReportePorFechasAsync(fechaDesde, fechaHasta);
            return View(filas);
        }
    }
}
