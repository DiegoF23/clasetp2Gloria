using System;

namespace GestionStockVentasBD
{
    // HERENCIA:
    // Lavarropas hereda de Producto y agrega datos específicos.
    public class Lavarropas : Producto
    {
        public int CargaKg { get; set; }
        public string Tipo { get; set; }

        public Lavarropas()
        {
        }

        public Lavarropas(
            int idProducto,
            int codigo,
            string nombre,
            decimal precio,
            int stock,
            int idSucursal,
            int cargaKg,
            string tipo
        ) : base(idProducto, codigo, nombre, precio, stock, idSucursal)
        {
            CargaKg = cargaKg;
            Tipo = tipo;
        }

        // POLIMORFISMO:
        // Lavarropas calcula el precio final con su propia regla.
        public override decimal CalcularPrecioFinal()
        {
            return Precio * 1.08m;
        }

        public override void MostrarInformacion()
        {
            base.MostrarInformacion();
            Console.WriteLine("Tipo: Lavarropas");
            Console.WriteLine("Carga: " + CargaKg + " kg");
            Console.WriteLine("Tipo de lavarropas: " + Tipo);
        }
    }
}