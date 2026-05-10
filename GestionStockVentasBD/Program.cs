using System;

namespace GestionStockVentasBD
{
    class Program
    {
        static void Main(string[] args)
        {
            int contador = 0;
            bool bandera = true;
            Console.WriteLine("conteo");
            Console.WriteLine("*********");

            Console.WriteLine("");
            while (bandera) 
            {
                contador++;
                Console.WriteLine(contador);
                if (contador > 15000)
                {
                    bandera = false;
                }
            }
            Console.WriteLine("el conteo finalizo ........");

            Console.ReadKey();
        }
    }
}