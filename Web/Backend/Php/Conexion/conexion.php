<?php

declare (strict_types=1);

CONST DB_HOST = 'localhost';
CONST DB_NAME = 'db_usuarios';
CONST DB_USER = 'root';
CONST DB_PASS = '';

function db() : PDO {
    



    try {
        $conexion = new PDO('mysql:host=' . DB_HOST . ';dbname=' . DB_NAME, DB_USER, DB_PASS);
        $conexion->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    } catch (PDOException $e) {
        throw new Exception('Error de conexión a la base de datos: ' . $e->getMessage());
    }

}

?>