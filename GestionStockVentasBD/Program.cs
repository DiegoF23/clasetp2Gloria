using System;
using System.Collections.Generic;

namespace GestionStockVentasBD
{
    class Program
    {
        static void Main(string[] args)
        {
            // Program.cs es la parte visible para el usuario.
            // No tiene consultas SQL directas: delega ese trabajo en los repositorios.
            ProductoRepositorio productoRepo = new ProductoRepositorio();
            SucursalRepositorio sucursalRepo = new SucursalRepositorio();
            VentaRepositorio ventaRepo = new VentaRepositorio();

            try
            {
                int idSucursal = SeleccionarSucursal(sucursalRepo);
                bool continuar = true;

                // Este while mantiene vivo el menu hasta que el usuario elige salir.
                while (continuar)
                {
                    Console.WriteLine();
                    Console.WriteLine("Sucursal seleccionada: " + idSucursal);
                    Console.WriteLine("Seleccione accion:");
                    Console.WriteLine("1 - Listar productos");
                    Console.WriteLine("2 - Agregar producto");
                    Console.WriteLine("3 - Modificar producto");
                    Console.WriteLine("4 - Eliminar producto");
                    Console.WriteLine("5 - Vender producto");
                    Console.WriteLine("6 - Cambiar sucursal");
                    Console.WriteLine("0 - Salir");

                    string opcion = Console.ReadLine();

                    switch (opcion)
                    {
                        case "1":
                            ListarProductos(productoRepo, idSucursal);
                            break;

                        case "2":
                            AgregarProductoDesdeMenu(productoRepo, idSucursal);
                            break;

                        case "3":
                            ModificarProductoDesdeMenu(productoRepo, idSucursal);
                            break;

                        case "4":
                            EliminarProductoDesdeMenu(productoRepo, idSucursal);
                            break;

                        case "5":
                            VenderProductoDesdeMenu(ventaRepo, idSucursal);
                            break;

                        case "6":
                            idSucursal = SeleccionarSucursal(sucursalRepo);
                            break;

                        case "0":
                            continuar = false;
                            break;

                        default:
                            Console.WriteLine("Opcion invalida.");
                            break;
                    }
                }

                Console.WriteLine("Programa finalizado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrio un error general.");
                Console.WriteLine("Motivo: " + ex.Message);
            }

            Console.ReadKey();
        }

        static int SeleccionarSucursal(SucursalRepositorio repo)
        {
            // Primero se leen las sucursales desde la base para no escribirlas fijas en el codigo.
            List<Sucursal> sucursales = repo.ObtenerSucursales();

            if (sucursales.Count == 0)
            {
                throw new Exception("No hay sucursales cargadas en la base de datos.");
            }

            while (true)
            {
                Console.WriteLine("Seleccione sucursal:");

                foreach (Sucursal sucursal in sucursales)
                {
                    Console.WriteLine(sucursal.IdSucursal + " - " + sucursal.Nombre);
                }

                int idSucursal = LeerEntero("Ingrese ID de sucursal:");

                foreach (Sucursal sucursal in sucursales)
                {
                    if (sucursal.IdSucursal == idSucursal)
                    {
                        return idSucursal;
                    }
                }

                Console.WriteLine("Sucursal invalida. Intente nuevamente.");
            }
        }

        static void ListarProductos(ProductoRepositorio repo, int idSucursal)
        {
            // El listado demuestra polimorfismo: todos se recorren como Producto,
            // pero cada objeto muestra tambien sus datos particulares.
            List<Producto> productos = repo.ObtenerProductosPorSucursal(idSucursal);

            if (productos.Count == 0)
            {
                Console.WriteLine("No hay productos en esta sucursal.");
                return;
            }

            foreach (Producto producto in productos)
            {
                producto.MostrarInformacion();
                Console.WriteLine("----------------------");
            }
        }

        static void AgregarProductoDesdeMenu(ProductoRepositorio repo, int idSucursal)
        {
            Console.WriteLine();
            Console.WriteLine("Tipo de producto:");
            Console.WriteLine("1 - Televisor");
            Console.WriteLine("2 - Heladera");
            Console.WriteLine("3 - Lavarropas");

            string tipo = Console.ReadLine();

            int codigo = LeerEntero("Codigo:");
            string nombre = LeerTexto("Nombre:");
            decimal precio = LeerDecimal("Precio:");
            int stock = LeerEntero("Stock:");

            Producto producto = null;

            // Segun el tipo elegido se crea una clase hija distinta.
            // Eso cumple con herencia porque todas derivan de Producto.
            if (tipo == "1")
            {
                int pulgadas = LeerEntero("Pulgadas:");
                string tipoPantalla = LeerTexto("Tipo de pantalla:");

                producto = new Televisor(0, codigo, nombre, precio, stock, idSucursal, pulgadas, tipoPantalla);
            }
            else if (tipo == "2")
            {
                int capacidad = LeerEntero("Capacidad en litros:");
                string tipoHeladera = LeerTexto("Tipo de heladera:");

                producto = new Heladera(0, codigo, nombre, precio, stock, idSucursal, capacidad, tipoHeladera);
            }
            else if (tipo == "3")
            {
                int cargaKg = LeerEntero("Carga en kg:");
                string tipoLavarropas = LeerTexto("Tipo de lavarropas:");

                producto = new Lavarropas(0, codigo, nombre, precio, stock, idSucursal, cargaKg, tipoLavarropas);
            }
            else
            {
                Console.WriteLine("Tipo invalido.");
                return;
            }

            repo.AgregarProducto(producto);
        }

        static void ModificarProductoDesdeMenu(ProductoRepositorio repo, int idSucursal)
        {
            int codigo = LeerEntero("Ingrese el codigo del producto a modificar:");
            Producto producto = repo.ObtenerProductoPorCodigo(codigo, idSucursal);

            if (producto == null)
            {
                Console.WriteLine("No se encontro un producto con ese codigo.");
                return;
            }

            Console.WriteLine("Producto encontrado:");
            producto.MostrarInformacion();

            producto.Nombre = LeerTexto("Nuevo nombre:");
            producto.Precio = LeerDecimal("Nuevo precio:");
            producto.Stock = LeerEntero("Nuevo stock:");

            // Con as verificamos que clase hija es para pedir sus campos propios.
            Televisor televisor = producto as Televisor;

            if (televisor != null)
            {
                televisor.Pulgadas = LeerEntero("Nuevas pulgadas:");
                televisor.TipoPantalla = LeerTexto("Nuevo tipo de pantalla:");
            }

            Heladera heladera = producto as Heladera;

            if (heladera != null)
            {
                heladera.CapacidadLitros = LeerEntero("Nueva capacidad en litros:");
                heladera.Tipo = LeerTexto("Nuevo tipo de heladera:");
            }

            Lavarropas lavarropas = producto as Lavarropas;

            if (lavarropas != null)
            {
                lavarropas.CargaKg = LeerEntero("Nueva carga en kg:");
                lavarropas.Tipo = LeerTexto("Nuevo tipo de lavarropas:");
            }

            repo.ModificarProducto(producto);
        }

        static void EliminarProductoDesdeMenu(ProductoRepositorio repo, int idSucursal)
        {
            int codigo = LeerEntero("Ingrese el codigo del producto a eliminar:");
            repo.EliminarProducto(codigo, idSucursal);
        }

        static void VenderProductoDesdeMenu(VentaRepositorio repo, int idSucursal)
        {
            int codigo = LeerEntero("Ingrese codigo de producto:");
            int cantidad = LeerEntero("Cantidad a vender:");
            repo.RegistrarVenta(idSucursal, codigo, cantidad);
        }

        static int LeerEntero(string mensaje)
        {
            int valor;
            bool correcto = false;

            do
            {
                Console.WriteLine(mensaje);
                string entrada = Console.ReadLine();
                correcto = int.TryParse(entrada, out valor);

                if (correcto == false)
                {
                    Console.WriteLine("Debe ingresar un numero entero valido.");
                }
            } while (correcto == false);

            return valor;
        }

        static decimal LeerDecimal(string mensaje)
        {
            decimal valor;
            bool correcto = false;

            do
            {
                Console.WriteLine(mensaje);
                string entrada = Console.ReadLine();
                correcto = decimal.TryParse(entrada, out valor);

                if (correcto == false)
                {
                    Console.WriteLine("Debe ingresar un numero decimal valido.");
                }
            } while (correcto == false);

            return valor;
        }

        static string LeerTexto(string mensaje)
        {
            string texto;

            do
            {
                Console.WriteLine(mensaje);
                texto = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    Console.WriteLine("El texto no puede estar vacio.");
                }
            } while (string.IsNullOrWhiteSpace(texto));

            return texto;
        }
    }
}
