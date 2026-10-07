using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios
{
    /// <summary>
    /// Acceso a datos de Libros (y la lista de Autores) con Dapper.
    /// Todo se ejecuta mediante procedimientos almacenados con parámetros.
    /// </summary>
    public class LibroRepositorio : ILibroRepositorio
    {
        private readonly string _cadenaConexion;

        public LibroRepositorio(IConfiguration configuration)
        {
            _cadenaConexion = configuration.GetConnectionString("BibliotecaDB")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'BibliotecaDB' en appsettings.json.");
        }

        private SqlConnection CrearConexion() => new SqlConnection(_cadenaConexion);

        public async Task<IEnumerable<Libro>> ListarAsync(string? busqueda)
        {
            using var conexion = CrearConexion();

            if (string.IsNullOrWhiteSpace(busqueda))
            {
                return await conexion.QueryAsync<Libro>(
                    "sp_Libros_Listar",
                    commandType: CommandType.StoredProcedure);
            }

            return await conexion.QueryAsync<Libro>(
                "sp_Libros_Buscar",
                new { Filtro = busqueda.Trim() },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Libro?> ObtenerPorIdAsync(int libroId)
        {
            using var conexion = CrearConexion();

            var libro = await conexion.QuerySingleOrDefaultAsync<Libro>(
                "sp_Libros_ObtenerPorId",
                new { LibroId = libroId },
                commandType: CommandType.StoredProcedure);

            // El procedimiento devuelve también los libros dados de baja; aquí se ocultan.
            return libro is { Activo: true } ? libro : null;
        }

        public async Task<int> InsertarAsync(Libro libro)
        {
            using var conexion = CrearConexion();

            var parametros = new DynamicParameters();
            parametros.Add("@Titulo", libro.Titulo);
            parametros.Add("@ISBN", libro.ISBN);
            parametros.Add("@AutorId", libro.AutorId);
            parametros.Add("@Ejemplares", libro.Ejemplares);
            parametros.Add("@NuevoId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conexion.ExecuteAsync(
                "sp_Libros_Insertar",
                parametros,
                commandType: CommandType.StoredProcedure);

            return parametros.Get<int>("@NuevoId");
        }

        public async Task ActualizarAsync(Libro libro)
        {
            using var conexion = CrearConexion();

            await conexion.ExecuteAsync(
                "sp_Libros_Actualizar",
                new
                {
                    libro.LibroId,
                    libro.Titulo,
                    libro.ISBN,
                    libro.AutorId,
                    libro.Ejemplares
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>Eliminación lógica (Activo = 0). Nunca un DELETE físico.</summary>
        public async Task EliminarAsync(int libroId)
        {
            using var conexion = CrearConexion();

            await conexion.ExecuteAsync(
                "sp_Libros_EliminarLogico",
                new { LibroId = libroId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> ExisteIsbnAsync(string isbn, int libroId)
        {
            using var conexion = CrearConexion();

            int cantidad = await conexion.ExecuteScalarAsync<int>(
                "sp_Libros_ExisteISBN",
                new { ISBN = isbn, LibroId = libroId },
                commandType: CommandType.StoredProcedure);

            return cantidad > 0;
        }

        public async Task<bool> TienePrestamosPendientesAsync(int libroId)
        {
            using var conexion = CrearConexion();

            int cantidad = await conexion.ExecuteScalarAsync<int>(
                "sp_Libros_TienePrestamosPendientes",
                new { LibroId = libroId },
                commandType: CommandType.StoredProcedure);

            return cantidad > 0;
        }

        public async Task<IEnumerable<Autor>> ListarAutoresAsync()
        {
            using var conexion = CrearConexion();

            return await conexion.QueryAsync<Autor>(
                "sp_Autores_Listar",
                commandType: CommandType.StoredProcedure);
        }
    }
}
