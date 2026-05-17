using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GestionStockVentasBD
{
    public class SucursalRepositorio
    {
        private Conexion conexion = new Conexion();

        public List<Sucursal> ObtenerSucursales()
        {
            // Este repositorio se encarga solo de traer sucursales desde MySQL.
            // Program.cs lo usa para mostrar opciones al usuario.
            List<Sucursal> sucursales = new List<Sucursal>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
         

                string sql = "SELECT IdSucursal, Nombre FROM Sucursal";

                MySqlCommand comando = new MySqlCommand(sql, con);

                MySqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Sucursal sucursal = new Sucursal();

                    sucursal.IdSucursal = Convert.ToInt32(reader["IdSucursal"]);
                    sucursal.Nombre = reader["Nombre"].ToString();
                    sucursales.Add(sucursal);
                }

                reader.Close();
            }

            return sucursales;
        }
    }
}
