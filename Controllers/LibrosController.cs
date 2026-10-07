using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ILibroRepositorio _libros;

        public LibrosController(ILibroRepositorio libros)
        {
            _libros = libros;
        }

        // GET: /Libros?buscar=texto
        public async Task<IActionResult> Index(string? buscar)
        {
            ViewData["Buscar"] = buscar;
            var lista = await _libros.ListarAsync(buscar);
            return View(lista);
        }

        // GET: /Libros/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var libro = await _libros.ObtenerPorIdAsync(id);
            if (libro == null) return NotFound();

            return View(libro);
        }

        // GET: /Libros/Create
        public async Task<IActionResult> Create()
        {
            await CargarAutoresAsync();
            return View(new Libro { Ejemplares = 1 });
        }

        // POST: /Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro)
        {
            if (ModelState.IsValid && await _libros.ExisteIsbnAsync(libro.ISBN, 0))
            {
                ModelState.AddModelError(nameof(Libro.ISBN), "Ya existe un libro registrado con ese ISBN.");
            }

            if (!ModelState.IsValid)
            {
                await CargarAutoresAsync(libro.AutorId);
                return View(libro);
            }

            try
            {
                await _libros.InsertarAsync(libro);
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                // Clave única duplicada (otro usuario insertó el mismo ISBN entre la validación y el INSERT).
                ModelState.AddModelError(nameof(Libro.ISBN), "Ya existe un libro registrado con ese ISBN.");
                await CargarAutoresAsync(libro.AutorId);
                return View(libro);
            }

            TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Libros/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _libros.ObtenerPorIdAsync(id);
            if (libro == null) return NotFound();

            await CargarAutoresAsync(libro.AutorId);
            return View(libro);
        }

        // POST: /Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Libro libro)
        {
            if (id != libro.LibroId) return BadRequest();

            if (ModelState.IsValid && await _libros.ExisteIsbnAsync(libro.ISBN, libro.LibroId))
            {
                ModelState.AddModelError(nameof(Libro.ISBN), "Ya existe otro libro registrado con ese ISBN.");
            }

            if (!ModelState.IsValid)
            {
                await CargarAutoresAsync(libro.AutorId);
                return View(libro);
            }

            try
            {
                await _libros.ActualizarAsync(libro);
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                ModelState.AddModelError(nameof(Libro.ISBN), "Ya existe otro libro registrado con ese ISBN.");
                await CargarAutoresAsync(libro.AutorId);
                return View(libro);
            }

            TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Libros/Delete/5  (pantalla de confirmación)
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _libros.ObtenerPorIdAsync(id);
            if (libro == null) return NotFound();

            return View(libro);
        }

        // POST: /Libros/Delete/5  (eliminación lógica: Activo = 0)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _libros.ObtenerPorIdAsync(id);
            if (libro == null) return NotFound();

            if (await _libros.TienePrestamosPendientesAsync(id))
            {
                TempData["Error"] = $"No se puede eliminar \"{libro.Titulo}\": tiene préstamos pendientes de devolución.";
                return RedirectToAction(nameof(Index));
            }

            await _libros.EliminarAsync(id);

            TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se eliminó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Llena la lista desplegable de autores que usan Create y Edit.
        private async Task CargarAutoresAsync(int? autorSeleccionado = null)
        {
            var autores = await _libros.ListarAutoresAsync();
            ViewData["Autores"] = new SelectList(autores, nameof(Autor.AutorId), nameof(Autor.Nombre), autorSeleccionado);
        }
    }
}
