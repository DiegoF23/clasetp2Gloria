using System;
using System.Collections.Generic;

namespace GestionStockVentasBD
{
    class Program
    {
        static void Main(string[] args)
        {
            SucursalRepositorio repo = new SucursalRepositorio();
            List<Sucursal> sucursales = repo.ObtenerSucursales();
            Console.WriteLine("lista de Sucursales");
            Console.WriteLine("********************");
            foreach (Sucursal sucursal in sucursales)
            {
                Console.WriteLine(sucursal.IdSucursal + " - " + sucursal.Nombre);
            }
            Console.ReadKey();
        }
    }
}