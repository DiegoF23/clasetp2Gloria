using System;

namespace GestionStockVentasBD
{
    // ABSTRACCIÓN:
    // Producto representa lo común de todos los productos.
    // No se instancia directamente porque no vendemos un "Producto" genérico,
    // sino productos concretos: Televisor, Heladera o Lavarropas.
    public abstract class Producto
    {
        // Datos comunes que vienen de la tabla Producto.
        public int IdProducto { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int IdSucursal { get; set; }

        public Producto()
        {
        }

        public Producto(int idProducto, int codigo, string nombre, decimal precio, int stock, int idSucursal)
        {
            IdProducto = idProducto;
            Codigo = codigo;
            Nombre = nombre;
            Precio = precio;
            Stock = stock;
            IdSucursal = idSucursal;
        }

        // POLIMORFISMO:
        // Cada tipo de producto va a calcular su precio final de forma diferente.
        public abstract decimal CalcularPrecioFinal();

        public virtual void MostrarInformacion()
        {
            Console.WriteLine("ID Producto: " + IdProducto);
            Console.WriteLine("Código: " + Codigo);
            Console.WriteLine("Nombre: " + Nombre);
            Console.WriteLine("Precio base: $" + Precio);
            Console.WriteLine("Precio final: $" + CalcularPrecioFinal());
            Console.WriteLine("Stock: " + Stock);
        }
    }
}