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
            // Lee productos de una sucursal.
            // Se usa LEFT JOIN porque los datos comunes estan en Producto
            // y los datos particulares estan en Televisor, Heladera o Lavarropas.
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

                reader.Close();
            }

            return productos;
        }

        public Producto ObtenerProductoPorCodigo(int codigo, int idSucursal)
        {
            Producto producto = null;

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
                    producto = CrearProductoDesdeReader(reader);
                }

                reader.Close();
            }

            return producto;
        }

        private Producto CrearProductoDesdeReader(MySqlDataReader reader)
        {
            // Convierte una fila de MySQL en un objeto C#.
            // Aca aparece el polimorfismo: el metodo devuelve Producto,
            // pero realmente crea Televisor, Heladera o Lavarropas.
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
            // La base guarda el tipo como texto en la columna TipoProducto.
            // Este metodo traduce la clase C# al valor que entiende MySQL.
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

        public void AgregarProducto(Producto producto)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                // Se usa transaccion porque insertar un producto requiere dos pasos:
                // 1) insertar datos comunes en Producto
                // 2) insertar datos especificos en la tabla hija correspondiente
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    string tipoProducto = ObtenerTipoProducto(producto);

                    string sqlProducto = @"
                        INSERT INTO Producto (Codigo, Nombre, Precio, Stock, TipoProducto, IdSucursal)
                        VALUES (@codigo, @nombre, @precio, @stock, @tipoProducto, @idSucursal)";

                    MySqlCommand comandoProducto = new MySqlCommand(sqlProducto, con, transaccion);
                    comandoProducto.Parameters.AddWithValue("@codigo", producto.Codigo);
                    comandoProducto.Parameters.AddWithValue("@nombre", producto.Nombre);
                    comandoProducto.Parameters.AddWithValue("@precio", producto.Precio);
                    comandoProducto.Parameters.AddWithValue("@stock", producto.Stock);
                    comandoProducto.Parameters.AddWithValue("@tipoProducto", tipoProducto);
                    comandoProducto.Parameters.AddWithValue("@idSucursal", producto.IdSucursal);

                    comandoProducto.ExecuteNonQuery();

                    producto.IdProducto = Convert.ToInt32(comandoProducto.LastInsertedId);
                    InsertarDatosEspecificos(producto, con, transaccion);

                    transaccion.Commit();
                    Console.WriteLine("Producto agregado correctamente.");
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    Console.WriteLine("No se pudo agregar el producto.");
                    Console.WriteLine("Motivo: " + ex.Message);
                }
            }
        }

        private void InsertarDatosEspecificos(Producto producto, MySqlConnection con, MySqlTransaction transaccion)
        {
            Televisor televisor = producto as Televisor;

            if (televisor != null)
            {
                string sql = @"
                    INSERT INTO Televisor (IdProducto, Pulgadas, TipoPantalla)
                    VALUES (@idProducto, @pulgadas, @tipoPantalla)";

                MySqlCommand comando = new MySqlCommand(sql, con, transaccion);
                comando.Parameters.AddWithValue("@idProducto", televisor.IdProducto);
                comando.Parameters.AddWithValue("@pulgadas", televisor.Pulgadas);
                comando.Parameters.AddWithValue("@tipoPantalla", televisor.TipoPantalla);
                comando.ExecuteNonQuery();
                return;
            }

            Heladera heladera = producto as Heladera;

            if (heladera != null)
            {
                string sql = @"
                    INSERT INTO Heladera (IdProducto, CapacidadLitros, Tipo)
                    VALUES (@idProducto, @capacidadLitros, @tipo)";

                MySqlCommand comando = new MySqlCommand(sql, con, transaccion);
                comando.Parameters.AddWithValue("@idProducto", heladera.IdProducto);
                comando.Parameters.AddWithValue("@capacidadLitros", heladera.CapacidadLitros);
                comando.Parameters.AddWithValue("@tipo", heladera.Tipo);
                comando.ExecuteNonQuery();
                return;
            }

            Lavarropas lavarropas = producto as Lavarropas;

            if (lavarropas != null)
            {
                string sql = @"
                    INSERT INTO Lavarropas (IdProducto, CargaKg, Tipo)
                    VALUES (@idProducto, @cargaKg, @tipo)";

                MySqlCommand comando = new MySqlCommand(sql, con, transaccion);
                comando.Parameters.AddWithValue("@idProducto", lavarropas.IdProducto);
                comando.Parameters.AddWithValue("@cargaKg", lavarropas.CargaKg);
                comando.Parameters.AddWithValue("@tipo", lavarropas.Tipo);
                comando.ExecuteNonQuery();
                return;
            }

            throw new Exception("No se pudieron insertar los datos especificos del producto.");
        }

        public void ModificarProducto(Producto producto)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                // Tambien se usa transaccion al modificar para mantener sincronizadas
                // la tabla Producto y la tabla especifica del tipo.
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    string sqlProducto = @"
                        UPDATE Producto
                        SET Nombre = @nombre,
                            Precio = @precio,
                            Stock = @stock
                        WHERE IdProducto = @idProducto";

                    MySqlCommand comandoProducto = new MySqlCommand(sqlProducto, con, transaccion);
                    comandoProducto.Parameters.AddWithValue("@nombre", producto.Nombre);
                    comandoProducto.Parameters.AddWithValue("@precio", producto.Precio);
                    comandoProducto.Parameters.AddWithValue("@stock", producto.Stock);
                    comandoProducto.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                    comandoProducto.ExecuteNonQuery();

                    ModificarDatosEspecificos(producto, con, transaccion);

                    transaccion.Commit();
                    Console.WriteLine("Producto modificado correctamente.");
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    Console.WriteLine("No se pudo modificar el producto.");
                    Console.WriteLine("Motivo: " + ex.Message);
                }
            }
        }

        private void ModificarDatosEspecificos(Producto producto, MySqlConnection con, MySqlTransaction transaccion)
        {
            Televisor televisor = producto as Televisor;

            if (televisor != null)
            {
                string sql = @"
                    UPDATE Televisor
                    SET Pulgadas = @pulgadas,
                        TipoPantalla = @tipoPantalla
                    WHERE IdProducto = @idProducto";

                MySqlCommand comando = new MySqlCommand(sql, con, transaccion);
                comando.Parameters.AddWithValue("@pulgadas", televisor.Pulgadas);
                comando.Parameters.AddWithValue("@tipoPantalla", televisor.TipoPantalla);
                comando.Parameters.AddWithValue("@idProducto", televisor.IdProducto);
                comando.ExecuteNonQuery();
                return;
            }

            Heladera heladera = producto as Heladera;

            if (heladera != null)
            {
                string sql = @"
                    UPDATE Heladera
                    SET CapacidadLitros = @capacidadLitros,
                        Tipo = @tipo
                    WHERE IdProducto = @idProducto";

                MySqlCommand comando = new MySqlCommand(sql, con, transaccion);
                comando.Parameters.AddWithValue("@capacidadLitros", heladera.CapacidadLitros);
                comando.Parameters.AddWithValue("@tipo", heladera.Tipo);
                comando.Parameters.AddWithValue("@idProducto", heladera.IdProducto);
                comando.ExecuteNonQuery();
                return;
            }

            Lavarropas lavarropas = producto as Lavarropas;

            if (lavarropas != null)
            {
                string sql = @"
                    UPDATE Lavarropas
                    SET CargaKg = @cargaKg,
                        Tipo = @tipo
                    WHERE IdProducto = @idProducto";

                MySqlCommand comando = new MySqlCommand(sql, con, transaccion);
                comando.Parameters.AddWithValue("@cargaKg", lavarropas.CargaKg);
                comando.Parameters.AddWithValue("@tipo", lavarropas.Tipo);
                comando.Parameters.AddWithValue("@idProducto", lavarropas.IdProducto);
                comando.ExecuteNonQuery();
                return;
            }

            throw new Exception("No se pudieron modificar los datos especificos.");
        }

        public void EliminarProducto(int codigo, int idSucursal)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                try
                {
                    // El DELETE se hace sobre Producto.
                    // Las tablas especificas se borran solas por ON DELETE CASCADE.
                    string sql = @"
                        DELETE FROM Producto
                        WHERE Codigo = @codigo
                        AND IdSucursal = @idSucursal";

                    MySqlCommand comando = new MySqlCommand(sql, con);
                    comando.Parameters.AddWithValue("@codigo", codigo);
                    comando.Parameters.AddWithValue("@idSucursal", idSucursal);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        Console.WriteLine("Producto eliminado correctamente.");
                    }
                    else
                    {
                        Console.WriteLine("No se encontro un producto con ese codigo en la sucursal seleccionada.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se pudo eliminar el producto.");
                    Console.WriteLine("Motivo: " + ex.Message);
                    Console.WriteLine("Nota: si el producto ya tiene ventas, la base puede impedir eliminarlo por integridad referencial.");
                }
            }
        }
    }
}
