using MySql.Data.MySqlClient;

namespace GestionStockVentasBD
{
    public class Conexion
    {
        private string cadenaConexion = "server=localhost;database=ElectrodomesticosDB;uid=root;pwd=root;";

        public MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(cadenaConexion);
        }
    }
}