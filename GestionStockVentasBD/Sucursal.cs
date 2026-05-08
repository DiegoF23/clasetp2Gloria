namespace GestionStockVentasBD
{
    public class Sucursal
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }

        public Sucursal()
        {
        }

        public Sucursal(int idSucursal, string nombre)
        {
            IdSucursal = idSucursal;
            Nombre = nombre;
        }
    }
}