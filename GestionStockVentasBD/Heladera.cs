using System;

namespace GestionStockVentasBD
{
    // HERENCIA:
    // Heladera hereda de Producto y agrega datos propios.
    public class Heladera : Producto
    {
        public int CapacidadLitros { get; set; }
        public string Tipo { get; set; }

        public Heladera()
        {
        }

        public Heladera(
            int idProducto,
            int codigo,
            string nombre,
            decimal precio,
            int stock,
            int idSucursal,
            int capacidadLitros,
            string tipo
        ) : base(idProducto, codigo, nombre, precio, stock, idSucursal)
        {
            CapacidadLitros = capacidadLitros;
            Tipo = tipo;
        }

        // POLIMORFISMO:
        // Heladera calcula el precio final con otro recargo.
        public override decimal CalcularPrecioFinal()
        {
            return Precio * 1.15m;
        }

        public override void MostrarInformacion()
        {
            base.MostrarInformacion();
            Console.WriteLine("Tipo: Heladera");
            Console.WriteLine("Capacidad: " + CapacidadLitros + " litros");
            Console.WriteLine("Tipo de heladera: " + Tipo);
        }
    }
}