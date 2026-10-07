using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Controllers
{
    public class SociosController : Controller
    {
        private readonly ISocioRepositorio _socios;

        public SociosController(ISocioRepositorio socios)
        {
            _socios = socios;
        }

        // GET: /Socios
        public async Task<IActionResult> Index()
        {
            var lista = await _socios.ListarAsync();
            return View(lista);
        }

        // GET: /Socios/Create
        public IActionResult Create()
        {
            return View(new Socio());
        }

        // POST: /Socios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Socio socio)
        {
            if (ModelState.IsValid && await _socios.ExisteDniAsync(socio.DNI))
            {
                ModelState.AddModelError(nameof(Socio.DNI), "Ya existe un socio registrado con ese DNI.");
            }

            if (!ModelState.IsValid)
            {
                return View(socio);
            }

            try
            {
                await _socios.InsertarAsync(socio);
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                // Respaldo ante una condición de carrera: el DNI se insertó justo antes que este.
                ModelState.AddModelError(nameof(Socio.DNI), "Ya existe un socio registrado con ese DNI.");
                return View(socio);
            }

            TempData["Mensaje"] = $"El socio \"{socio.Nombre}\" se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
