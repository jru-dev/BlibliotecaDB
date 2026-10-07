using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios
{
    public class SocioRepositorio : ISocioRepositorio
    {
        private readonly string _cadenaConexion;

        public SocioRepositorio(IConfiguration configuration)
        {
            _cadenaConexion = configuration.GetConnectionString("BibliotecaDB")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'BibliotecaDB' en appsettings.json.");
        }

        private SqlConnection CrearConexion() => new SqlConnection(_cadenaConexion);

        public async Task<IEnumerable<Socio>> ListarAsync()
        {
            using var conexion = CrearConexion();

            return await conexion.QueryAsync<Socio>(
                "sp_Socios_Listar",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertarAsync(Socio socio)
        {
            using var conexion = CrearConexion();

            var parametros = new DynamicParameters();
            parametros.Add("@DNI", socio.DNI);
            parametros.Add("@Nombre", socio.Nombre);
            parametros.Add("@Email", socio.Email);
            parametros.Add("@NuevoId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conexion.ExecuteAsync(
                "sp_Socios_Insertar",
                parametros,
                commandType: CommandType.StoredProcedure);

            return parametros.Get<int>("@NuevoId");
        }

        public async Task<bool> ExisteDniAsync(string dni)
        {
            using var conexion = CrearConexion();

            // @SocioId = 0 (valor por defecto): busca el DNI en cualquier socio.
            int cantidad = await conexion.ExecuteScalarAsync<int>(
                "sp_Socios_ExisteDNI",
                new { DNI = dni },
                commandType: CommandType.StoredProcedure);

            return cantidad > 0;
        }
    }
}
