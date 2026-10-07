using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios
{
    public class PrestamoRepositorio : IPrestamoRepositorio
    {
        private readonly string _cadenaConexion;

        public PrestamoRepositorio(IConfiguration configuration)
        {
            _cadenaConexion = configuration.GetConnectionString("BibliotecaDB")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'BibliotecaDB' en appsettings.json.");
        }

        private SqlConnection CrearConexion() => new SqlConnection(_cadenaConexion);

        public async Task<IEnumerable<PrestamoReporte>> ReportePorFechasAsync(DateTime desde, DateTime hasta)
        {
            using var conexion = CrearConexion();

            return await conexion.QueryAsync<PrestamoReporte>(
                "sp_Prestamos_ReportePorFechas",
                new { FechaInicio = desde.Date, FechaFin = hasta.Date },
                commandType: CommandType.StoredProcedure);
        }
    }
}
