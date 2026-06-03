CREATE DATABASE IF NOT EXISTS Lab1Ciber;
USE Lab1Ciber;

-- Eliminar tablas antiguas si existen (para empezar limpio)
DROP TABLE IF EXISTS `USERS`;
DROP TABLE IF EXISTS `PRODUCTS`;

-- Tabla de historial de migraciones de Entity Framework
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tablas de ASP.NET Core Identity
CREATE TABLE IF NOT EXISTS `AspNetUsers` (
    `Id` varchar(255) NOT NULL,
    `UserName` varchar(256) DEFAULT NULL,
    `NormalizedUserName` varchar(256) DEFAULT NULL,
    `Email` varchar(256) DEFAULT NULL,
    `NormalizedEmail` varchar(256) DEFAULT NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext DEFAULT NULL,
    `SecurityStamp` longtext DEFAULT NULL,
    `ConcurrencyStamp` longtext DEFAULT NULL,
    `PhoneNumber` longtext DEFAULT NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    `LockoutEnd` datetime(6) DEFAULT NULL,
    `LockoutEnabled` tinyint(1) NOT NULL,
    `AccessFailedCount` int NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
    KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla de Productos actualizada con la columna 'Category'
CREATE TABLE IF NOT EXISTS `Products` (
    `IdProduct` int NOT NULL AUTO_INCREMENT,
    `ImagenProduct` varchar(255) NOT NULL,
    `NameProduct` varchar(50) NOT NULL,
    `Descrip` longtext NOT NULL,
    `Price` decimal(20,4) NOT NULL,
    `Category` varchar(50) NOT NULL,
    PRIMARY KEY (`IdProduct`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
