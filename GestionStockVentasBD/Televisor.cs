using System;

namespace GestionStockVentasBD
{
    // HERENCIA:
    // Televisor hereda de Producto.
    // Por eso ya tiene IdProducto, Codigo, Nombre, Precio, Stock e IdSucursal.
    public class Televisor : Producto
    {
        public int Pulgadas { get; set; }
        public string TipoPantalla { get; set; }

        public Televisor()
        {
        }

        public Televisor(
            int idProducto,
            int codigo,
            string nombre,
            decimal precio,
            int stock,
            int idSucursal,
            int pulgadas,
            string tipoPantalla
        ) : base(idProducto, codigo, nombre, precio, stock, idSucursal)
        {
            // base(...) llama al constructor de Producto
            // y carga los datos comunes.
            Pulgadas = pulgadas;
            TipoPantalla = tipoPantalla;
        }

        // POLIMORFISMO:
        // Televisor implementa CalcularPrecioFinal() con su propia regla.
        public override decimal CalcularPrecioFinal()
        {
            return Precio * 1.10m;
        }

        public override void MostrarInformacion()
        {
            base.MostrarInformacion();
            Console.WriteLine("Tipo: Televisor");
            Console.WriteLine("Pulgadas: " + Pulgadas);
            Console.WriteLine("Tipo de pantalla: " + TipoPantalla);
        }
    }
}