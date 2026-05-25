# Ficha Técnica - Comidasa MVC (Seguridad de Software)

## Descripción del Proyecto
Comidasa es una aplicación web transaccional para un restaurante, desarrollada con un enfoque estricto en **Seguridad de Software**. El proyecto ha sido refactorizado desde una estructura PHP/HTML básica hacia una arquitectura empresarial **Modelo-Vista-Controlador (MVC)** utilizando el framework moderno **ASP.NET Core (C#)**.

## Arquitectura y Tecnologías
- **Backend:** C# / ASP.NET Core 9.0 MVC.
- **Frontend:** Razor Pages (`.cshtml`), HTML5, CSS3 nativo (Vanilla CSS) con diseño Glassmorphism y UI responsiva.
- **Base de Datos:** SQLite (Configurable fácilmente para MySQL) a través de **Entity Framework Core** (ORM).
- **Gestión de Identidad:** ASP.NET Core Identity.

## Controles de Seguridad Implementados (OWASP Top 10)

El proyecto mitiga proactivamente las vulnerabilidades más comunes identificadas por OWASP:

### 1. Inyección SQL (SQLi)
**Problema mitigado:** Ejecución de comandos maliciosos en la base de datos a través de entradas del usuario.
**Solución implementada:** Se prohibieron las consultas directas (`raw SQL`) y las llamadas inseguras. Toda comunicación con la base de datos se realiza a través de **Entity Framework Core (EF Core)**, el cual genera automáticamente **consultas parametrizadas**. Esto separa estructuralmente el comando de los datos ingresados por el usuario, haciendo que los payloads maliciosos sean tratados estrictamente como texto, anulando la inyección.

### 2. Cross-Site Scripting (XSS)
**Problema mitigado:** Inyección de scripts maliciosos (ej. JavaScript) en las vistas del Frontend.
**Solución implementada:** 
- **Razor HTML-Encoding:** El motor de plantillas de ASP.NET Core (`Razor`) codifica automáticamente (HTML-encode) cualquier valor o variable que se imprima en pantalla. Si un usuario introduce `<script>alert('hack')</script>`, se renderiza como texto inofensivo.
- **Content Security Policy (CSP):** Se implementó un middleware global en `Program.cs` que inyecta la cabecera `Content-Security-Policy`, la cual bloquea explícitamente la carga y ejecución de scripts ajenos al origen de la aplicación.

### 3. Falsificación de Petición en Sitios Cruzados (CSRF)
**Problema mitigado:** Ataques donde un usuario autenticado es engañado para enviar peticiones no deseadas a la aplicación.
**Solución implementada:** Se utilizan de manera nativa los **Tokens Anti-Falsificación (Anti-Forgery Tokens)**. Cada formulario generado en Razor incluye un token único (`__RequestVerificationToken`) que se valida obligatoriamente en el servidor. El backend rechazará solicitudes provenientes de scripts externos o dominios de terceros.

### 4. Autenticación y Gestión de Sesiones Rota
**Problema mitigado:** Robo de credenciales, almacenamiento inseguro de contraseñas.
**Solución implementada:** Se integró **ASP.NET Core Identity**:
- **Hashing Robusto:** Las contraseñas nunca se almacenan en texto plano. Se emplea PBKDF2 (Password-Based Key Derivation Function 2) con HMAC-SHA256, con generación de *sal* única por usuario y miles de iteraciones.
- **Autenticación de 2 Pasos (2FA):** Sistema preparado para habilitar tokens temporales de tiempo (TOTP) o correo electrónico como segunda capa de seguridad.
- **Recuperación Segura:** La funcionalidad de recuperación de contraseñas no da información del estado de la cuenta y usa tokens limitados en el tiempo.
- **Validación de Contraseñas:** Se fuerzan políticas estrictas (uso de caracteres especiales, números, mayúsculas y longitud mínima).

### 5. Configuraciones y Cabeceras de Seguridad
En la inicialización del Request Pipeline, se agregaron cabeceras HTTP de seguridad:
- `X-Frame-Options: DENY`: Evita ataques de **Clickjacking** al prohibir que la aplicación sea embebida en `<iframe>` o `<frame>` externos.
- `X-Content-Type-Options: nosniff`: Instruye al navegador a no intentar adivinar los tipos MIME, forzando la interpretación declarada, lo cual reduce ataques basados en descargas de archivos ocultos.
- `HSTS (HTTP Strict Transport Security)`: Fuerza que toda la comunicación sea estrictamente por canal cifrado HTTPS, evitando ataques de tipo *Man-in-the-Middle* y *Downgrade*.

## Guía de Despliegue Local
1. Navegar a la carpeta del proyecto `Comidasa`.
2. Restaurar dependencias y compilar: `dotnet build`
3. Iniciar la aplicación y el servidor Kestrel integrado: `dotnet run`
4. Acceder vía navegador (HTTPS predeterminado activado para garantizar cifrado de tránsito).
