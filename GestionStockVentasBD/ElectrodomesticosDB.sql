-- Base de datos del TP2: Sistema de Gestion de Stock y Ventas
-- Ejecutar este script en MySQL antes de usar la aplicacion.

DROP DATABASE IF EXISTS ElectrodomesticosDB;
CREATE DATABASE ElectrodomesticosDB;
USE ElectrodomesticosDB;

-- Sucursal guarda las dos sedes de la empresa: Centro y Norte.
CREATE TABLE Sucursal (
    IdSucursal INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

-- Producto guarda los datos comunes de todos los electrodomesticos.
CREATE TABLE Producto (
    IdProducto INT AUTO_INCREMENT PRIMARY KEY,
    Codigo INT NOT NULL UNIQUE,
    Nombre VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    TipoProducto ENUM('Televisor', 'Heladera', 'Lavarropas') NOT NULL,
    IdSucursal INT NOT NULL,
    FOREIGN KEY (IdSucursal) REFERENCES Sucursal(IdSucursal)
);

-- Tablas especificas: guardan solo los datos propios de cada tipo.
CREATE TABLE Televisor (
    IdProducto INT PRIMARY KEY,
    Pulgadas INT NOT NULL,
    TipoPantalla VARCHAR(50) NOT NULL,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
        ON DELETE CASCADE
);

CREATE TABLE Heladera (
    IdProducto INT PRIMARY KEY,
    CapacidadLitros INT NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
        ON DELETE CASCADE
);

CREATE TABLE Lavarropas (
    IdProducto INT PRIMARY KEY,
    CargaKg INT NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
        ON DELETE CASCADE
);

-- Venta es la cabecera de la operacion.
CREATE TABLE Venta (
    IdVenta INT AUTO_INCREMENT PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IdSucursal INT NOT NULL,
    FOREIGN KEY (IdSucursal) REFERENCES Sucursal(IdSucursal)
);

-- DetalleVenta indica que productos se vendieron y a que precio.
CREATE TABLE DetalleVenta (
    IdDetalle INT AUTO_INCREMENT PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta)
        ON DELETE CASCADE,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
);

INSERT INTO Sucursal (Nombre) VALUES
('Centro'),
('Norte');

INSERT INTO Producto (Codigo, Nombre, Precio, Stock, TipoProducto, IdSucursal) VALUES
(101, 'TV Samsung 50', 150000, 5, 'Televisor', 1),
(102, 'Heladera LG', 200000, 3, 'Heladera', 1),
(103, 'Lavarropas Drean', 180000, 4, 'Lavarropas', 2);

INSERT INTO Televisor (IdProducto, Pulgadas, TipoPantalla) VALUES
(1, 50, 'LED');

INSERT INTO Heladera (IdProducto, CapacidadLitros, Tipo) VALUES
(2, 350, 'No Frost');

INSERT INTO Lavarropas (IdProducto, CargaKg, Tipo) VALUES
(3, 7, 'Automatico');

-- Consulta de prueba para revisar que los datos iniciales esten cargados.
SELECT
    p.IdProducto,
    p.Codigo,
    p.Nombre,
    p.Precio,
    p.Stock,
    p.TipoProducto,
    s.Nombre AS Sucursal
FROM Producto p
JOIN Sucursal s ON p.IdSucursal = s.IdSucursal;
