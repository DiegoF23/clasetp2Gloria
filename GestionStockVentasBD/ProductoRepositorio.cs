using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;


namespace GestionStockVentasBD
{
    public class ProductoRepositorio
    {
        private Conexion conexion = new Conexion();

        public List<Producto> ObtenerProductosPorSucursal(int idSucursal)
        {
            List<Producto> productos = new List<Producto>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT 
                        p.IdProducto,
                        p.Codigo,
                        p.Nombre,
                        p.Precio,
                        p.Stock,
                        p.TipoProducto,
                        p.IdSucursal,
                        t.Pulgadas,
                        t.TipoPantalla,
                        h.CapacidadLitros,
                        h.Tipo AS TipoHeladera,
                        l.CargaKg,
                        l.Tipo AS TipoLavarropas
                    FROM Producto p
                    LEFT JOIN Televisor t ON p.IdProducto = t.IdProducto
                    LEFT JOIN Heladera h ON p.IdProducto = h.IdProducto
                    LEFT JOIN Lavarropas l ON p.IdProducto = l.IdProducto
                    WHERE p.IdSucursal = @idSucursal";

                MySqlCommand comando = new MySqlCommand(sql, con);
                comando.Parameters.AddWithValue("@idSucursal", idSucursal);

                MySqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Producto producto = CrearProductoDesdeReader(reader);

                    if (producto != null)
                    {
                        productos.Add(producto);
                    }
                }
            }

            return productos;
        }

        public Producto ObtenerProductoPorCodigo(int codigo, int idSucursal)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT 
                        p.IdProducto,
                        p.Codigo,
                        p.Nombre,
                        p.Precio,
                        p.Stock,
                        p.TipoProducto,
                        p.IdSucursal,
                        t.Pulgadas,
                        t.TipoPantalla,
                        h.CapacidadLitros,
                        h.Tipo AS TipoHeladera,
                        l.CargaKg,
                        l.Tipo AS TipoLavarropas
                    FROM Producto p
                    LEFT JOIN Televisor t ON p.IdProducto = t.IdProducto
                    LEFT JOIN Heladera h ON p.IdProducto = h.IdProducto
                    LEFT JOIN Lavarropas l ON p.IdProducto = l.IdProducto
                    WHERE p.Codigo = @codigo
                    AND p.IdSucursal = @idSucursal";

                MySqlCommand comando = new MySqlCommand(sql, con);
                comando.Parameters.AddWithValue("@codigo", codigo);
                comando.Parameters.AddWithValue("@idSucursal", idSucursal);

                MySqlDataReader reader = comando.ExecuteReader();

                if (reader.Read())
                {
                    return CrearProductoDesdeReader(reader);
                }

                return null;
            }
        }

        private Producto CrearProductoDesdeReader(MySqlDataReader reader)
        {
            int idProducto = Convert.ToInt32(reader["IdProducto"]);
            int codigo = Convert.ToInt32(reader["Codigo"]);
            string nombre = reader["Nombre"].ToString();
            decimal precio = Convert.ToDecimal(reader["Precio"]);
            int stock = Convert.ToInt32(reader["Stock"]);
            string tipoProducto = reader["TipoProducto"].ToString();
            int idSucursal = Convert.ToInt32(reader["IdSucursal"]);

            if (tipoProducto == "Televisor")
            {
                int pulgadas = Convert.ToInt32(reader["Pulgadas"]);
                string tipoPantalla = reader["TipoPantalla"].ToString();

                return new Televisor(
                    idProducto,
                    codigo,
                    nombre,
                    precio,
                    stock,
                    idSucursal,
                    pulgadas,
                    tipoPantalla
                );
            }

            if (tipoProducto == "Heladera")
            {
                int capacidad = Convert.ToInt32(reader["CapacidadLitros"]);
                string tipo = reader["TipoHeladera"].ToString();

                return new Heladera(
                    idProducto,
                    codigo,
                    nombre,
                    precio,
                    stock,
                    idSucursal,
                    capacidad,
                    tipo
                );
            }

            if (tipoProducto == "Lavarropas")
            {
                int carga = Convert.ToInt32(reader["CargaKg"]);
                string tipo = reader["TipoLavarropas"].ToString();

                return new Lavarropas(
                    idProducto,
                    codigo,
                    nombre,
                    precio,
                    stock,
                    idSucursal,
                    carga,
                    tipo
                );
            }

            return null;
        }

        private string ObtenerTipoProducto(Producto producto)
        {
            if (producto is Televisor)
            {
                return "Televisor";
            }

            if (producto is Heladera)
            {
                return "Heladera";
            }

            if (producto is Lavarropas)
            {
                return "Lavarropas";
            }

            throw new Exception("Tipo de producto no válido.");
        }
    }
}