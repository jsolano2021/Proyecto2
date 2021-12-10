create database Proyecto2
go

use Proyecto2
go

create table Perfil(
id integer not null primary key,
nombre varchar(50) not null
);

INSERT INTO Perfil(id, nombre) VALUES(1, 'Administrador'), (2, 'Vendedor'), (3, 'Usuario Final')-- O LO QUE SEA xD


create table Usuarios(
id integer not null primary key identity(1,1), 
idPerfil integer not null foreign key references Perfil(id),
nombre varchar(100) not null,
apellidos varchar(100) not null,
telefono varchar(20) not null,
correo varchar(100) not null,
userName varchar(100) not null UNIQUE,
password varchar(100) not null,
estado bit not null default(1)
);
go

Create table Restaurante(
id integer identity(1,1) not null primary key,
nombre varchar(100) not null,
direccion varchar(200) not null,
telefono varchar(20) not null,
estado bit not null default(1)
);


create table Menu(
id integer identity(1,1) not null primary key,
idRestaurante int not null foreign key references Restaurante (id),
nombre varchar(100) not null,
estado bit not null default(1)
);

create table Platillo(
id integer identity(1,1) not null primary key,
idMenu int not null foreign key references Menu (id),
nombre varchar(100) not null,
descripcion varchar(250) not null,
precio float not null,
Imagen image null,
stock float not null DEFAULT(1),
estado bit not null default(1),
);

Create table Factura(
id integer not null primary key identity(1,1),
idUsuario integer not null foreign key references Usuarios (id),
fecha datetime not null default(GETDATE()),
totalFactura float not null,
);

create table Detalle(
id integer not null primary key identity(1,1),
idFactura  integer not null foreign key references Factura (id),
idPlatillo integer not null foreign key references Platillo (id),
idRestaurante integer not null foreign key references Restaurante (id),
cantidad float not null,
precio float not null,
total float not null,
estado bit not null default(1),
);


INSERT INTO Usuarios (idPerfil, nombre, apellidos, UserName, password, telefono, correo)
values(1, 'Jose', 'Solano Garita', 'Admin', '12345', '8888-8888', 'ruso22121@gmail.com');

INSERT INTO Usuarios (idPerfil, nombre, apellidos, UserName, password, telefono, correo)
values(3, 'Johnny', 'Depp', 'Jdepp', '12345', '8888-8888', 'ruso22121@gmail.com');