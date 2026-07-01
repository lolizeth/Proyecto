CREATE DATABASE pos;
USE pos;

CREATE TABLE clientes(
 id_cliente INT PRIMARY KEY AUTO_INCREMENT,
 nombre VARCHAR(50),
 direccion VARCHAR(100),
 telefono VARCHAR(20),
 correo VARCHAR(50)
);

CREATE TABLE categoria(
 id_categoria INT PRIMARY KEY AUTO_INCREMENT,
 descripcion VARCHAR(50)
);

CREATE TABLE proveedores(
 id_proveedor INT PRIMARY KEY AUTO_INCREMENT,
 nombre VARCHAR(50),
 direccion VARCHAR(100),
 telefono VARCHAR(20),
 contacto VARCHAR(50),
 productosum VARCHAR(100),
 correo VARCHAR(50)
);

