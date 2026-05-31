using MySql.Data.MySqlClient;
using System.Configuration;

namespace GestionStockVentasBD
{
    public class Conexion
    {
        private string cadenaConexion;

        public Conexion()
        {
            cadenaConexion = ConfigurationManager.ConnectionStrings["ElectrodomesticosDB"].ConnectionString;
        }

        public MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(cadenaConexion);
        }
    }
}