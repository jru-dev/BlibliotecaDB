using Biblioteca.Web.Models;

namespace Biblioteca.Web.Repositorios
{
    public interface ILibroRepositorio
    {
        Task<IEnumerable<Libro>> ListarAsync(string? busqueda);
        Task<Libro?> ObtenerPorIdAsync(int libroId);
        Task<int> InsertarAsync(Libro libro);
        Task ActualizarAsync(Libro libro);
        Task EliminarAsync(int libroId);
        Task<bool> ExisteIsbnAsync(string isbn, int libroId);
        Task<bool> TienePrestamosPendientesAsync(int libroId);
        Task<IEnumerable<Autor>> ListarAutoresAsync();
    }
}
