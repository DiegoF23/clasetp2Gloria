using System.Collections.Generic;

namespace GestionStockVentasBD
{
    public class Sucursal
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }

        // COMPOSICION:
        // Una sucursal puede contener muchos productos.
        // En la base esa relacion se guarda con Producto.IdSucursal.
        public List<Producto> Productos { get; set; }

        public Sucursal()
        {
            Productos = new List<Producto>();
        }

        public Sucursal(int idSucursal, string nombre)
        {
            IdSucursal = idSucursal;
            Nombre = nombre;
            Productos = new List<Producto>();
        }
    }
}
