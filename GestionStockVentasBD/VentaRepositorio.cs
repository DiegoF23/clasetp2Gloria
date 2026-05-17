using System;
using MySql.Data.MySqlClient;

namespace GestionStockVentasBD
{
    public class VentaRepositorio
    {
        private Conexion conexion = new Conexion();

        public void RegistrarVenta(int idSucursal, int codigoProducto, int cantidad)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                // TRANSACCION:
                // La venta tiene varios pasos. Si uno falla, no debe quedar una venta incompleta.
                // Por eso se confirma con Commit o se deshace con Rollback.
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    Producto producto = ObtenerProductoParaVenta(con, transaccion, idSucursal, codigoProducto);

                    if (producto == null)
                    {
                        throw new Exception("Producto no encontrado.");
                    }

                    if (cantidad <= 0)
                    {
                        throw new Exception("La cantidad debe ser mayor a cero.");
                    }

                    if (producto.Stock < cantidad)
                    {
                        throw new Exception("Stock insuficiente.");
                    }

                    decimal precioFinal = producto.CalcularPrecioFinal();
                    decimal total = precioFinal * cantidad;

                    // 1) Se registra la cabecera de la venta.
                    string sqlVenta = @"
                        INSERT INTO Venta (IdSucursal)
                        VALUES (@idSucursal)";

                    MySqlCommand comandoVenta = new MySqlCommand(sqlVenta, con, transaccion);
                    comandoVenta.Parameters.AddWithValue("@idSucursal", idSucursal);
                    comandoVenta.ExecuteNonQuery();

                    int idVenta = Convert.ToInt32(comandoVenta.LastInsertedId);

                    // 2) Se registra el detalle: producto vendido, cantidad y precio unitario final.
                    string sqlDetalle = @"
                        INSERT INTO DetalleVenta (IdVenta, IdProducto, Cantidad, PrecioUnitario)
                        VALUES (@idVenta, @idProducto, @cantidad, @precioUnitario)";

                    MySqlCommand comandoDetalle = new MySqlCommand(sqlDetalle, con, transaccion);
                    comandoDetalle.Parameters.AddWithValue("@idVenta", idVenta);
                    comandoDetalle.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                    comandoDetalle.Parameters.AddWithValue("@cantidad", cantidad);
                    comandoDetalle.Parameters.AddWithValue("@precioUnitario", precioFinal);
                    comandoDetalle.ExecuteNonQuery();

                    // 3) Se descuenta stock. La condicion Stock >= @cantidad evita stock negativo.
                    string sqlStock = @"
                        UPDATE Producto
                        SET Stock = Stock - @cantidad
                        WHERE IdProducto = @idProducto
                        AND Stock >= @cantidad";

                    MySqlCommand comandoStock = new MySqlCommand(sqlStock, con, transaccion);
                    comandoStock.Parameters.AddWithValue("@cantidad", cantidad);
                    comandoStock.Parameters.AddWithValue("@idProducto", producto.IdProducto);

                    int filasStock = comandoStock.ExecuteNonQuery();

                    if (filasStock == 0)
                    {
                        throw new Exception("No se pudo actualizar el stock.");
                    }

                    transaccion.Commit();

                    Console.WriteLine("Venta realizada con exito.");
                    Console.WriteLine("Total: $" + total);
                    Console.WriteLine("Stock restante: " + (producto.Stock - cantidad));
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();

                    Console.WriteLine("No se pudo realizar la venta.");
                    Console.WriteLine("Motivo: " + ex.Message);
                }
            }
        }

        private Producto ObtenerProductoParaVenta(
            MySqlConnection con,
            MySqlTransaction transaccion,
            int idSucursal,
            int codigoProducto
        )
        {
            // FOR UPDATE bloquea la fila del producto mientras dura la transaccion.
            // Eso evita que dos ventas descuenten el mismo stock al mismo tiempo.
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
                AND p.IdSucursal = @idSucursal
                FOR UPDATE";

            MySqlCommand comando = new MySqlCommand(sql, con, transaccion);
            comando.Parameters.AddWithValue("@codigo", codigoProducto);
            comando.Parameters.AddWithValue("@idSucursal", idSucursal);

            MySqlDataReader reader = comando.ExecuteReader();

            if (reader.Read() == false)
            {
                reader.Close();
                return null;
            }

            int idProducto = Convert.ToInt32(reader["IdProducto"]);
            int codigo = Convert.ToInt32(reader["Codigo"]);
            string nombre = reader["Nombre"].ToString();
            decimal precio = Convert.ToDecimal(reader["Precio"]);
            int stock = Convert.ToInt32(reader["Stock"]);
            string tipoProducto = reader["TipoProducto"].ToString();
            int sucursal = Convert.ToInt32(reader["IdSucursal"]);

            Producto producto = null;

            if (tipoProducto == "Televisor")
            {
                int pulgadas = Convert.ToInt32(reader["Pulgadas"]);
                string tipoPantalla = reader["TipoPantalla"].ToString();
                producto = new Televisor(idProducto, codigo, nombre, precio, stock, sucursal, pulgadas, tipoPantalla);
            }
            else if (tipoProducto == "Heladera")
            {
                int capacidad = Convert.ToInt32(reader["CapacidadLitros"]);
                string tipo = reader["TipoHeladera"].ToString();
                producto = new Heladera(idProducto, codigo, nombre, precio, stock, sucursal, capacidad, tipo);
            }
            else if (tipoProducto == "Lavarropas")
            {
                int carga = Convert.ToInt32(reader["CargaKg"]);
                string tipo = reader["TipoLavarropas"].ToString();
                producto = new Lavarropas(idProducto, codigo, nombre, precio, stock, sucursal, carga, tipo);
            }

            reader.Close();
            return producto;
        }
    }
}
