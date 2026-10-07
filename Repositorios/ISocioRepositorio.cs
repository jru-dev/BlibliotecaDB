using Biblioteca.Web.Models;

namespace Biblioteca.Web.Repositorios
{
    public interface ISocioRepositorio
    {
        Task<IEnumerable<Socio>> ListarAsync();
        Task<int> InsertarAsync(Socio socio);
        Task<bool> ExisteDniAsync(string dni);
    }
}
