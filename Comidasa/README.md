# Ficha Técnica - Comidasa MVC (Seguridad de Software)

## Descripción del Proyecto
Comidasa es una aplicación web transaccional para un restaurante, desarrollada con un enfoque estricto en **Seguridad de Software**. El proyecto ha sido refactorizado desde una estructura PHP/HTML básica hacia una arquitectura empresarial **Modelo-Vista-Controlador (MVC)** utilizando el framework moderno **ASP.NET Core (C#)**.

## Arquitectura y Estructura de Carpetas (MVC)
La aplicación sigue estrictamente el patrón arquitectónico Modelo-Vista-Controlador para separar la lógica de negocio, la interfaz de usuario y las reglas de enrutamiento:

- 📂 **`Models/` (Modelos)**: Contiene las estructuras de datos (Ej. `Product.cs`). Aquí se define qué propiedades tiene cada elemento (nombre, precio, etc.) y cómo se comunican con la base de datos.
- 📂 **`Views/` (Vistas)**: Contiene la interfaz gráfica del usuario. Usa archivos `.cshtml` (C# + HTML / Razor).
  - `Home/`: Vistas públicas como el catálogo (`Index.cshtml`) y el detalle del producto (`Details.cshtml`).
  - `Shared/`: Componentes reutilizables como la barra de navegación (`_Layout.cshtml`) y botones de login (`_LoginPartial.cshtml`).
- 📂 **`Controllers/` (Controladores)**: Contiene la lógica de negocio (`HomeController.cs`). Son los intermediarios: reciben la petición web, buscan datos en los **Modelos** y se los entregan a las **Vistas** para ser mostrados.
- 📂 **`Areas/Identity/` (Gestión de Usuarios)**: Contiene el sistema de autenticación de ASP.NET Core Identity. Usa un patrón llamado **Razor Pages** donde la Vista (`.cshtml`) y su mini-controlador o Code-Behind (`.cshtml.cs`) están juntos para manejar flujos de seguridad (Login, Registro) de forma aislada.
- 📂 **`wwwroot/`**: Archivos públicos estáticos (Imágenes, CSS, JavaScript).

## Controles de Seguridad Implementados (OWASP Top 10)

El proyecto mitiga proactivamente las vulnerabilidades más comunes identificadas por OWASP:

### 1. Inyección SQL (SQLi)
**Problema mitigado:** Ejecución de comandos maliciosos en la base de datos a través de entradas del usuario.
**Solución implementada:** Se prohibieron las consultas directas (`raw SQL`) y las llamadas inseguras. Toda comunicación con la base de datos se realiza a través de **Entity Framework Core (EF Core)**, el cual genera automáticamente **consultas parametrizadas**.

### 2. Cross-Site Scripting (XSS)
**Problema mitigado:** Inyección de scripts maliciosos (ej. JavaScript) en las vistas del Frontend.
**Solución implementada:** 
- **Razor HTML-Encoding:** El motor de plantillas de ASP.NET Core (`Razor`) codifica automáticamente cualquier valor o variable que se imprima en pantalla.
- **Content Security Policy (CSP):** Se implementó un middleware global en `Program.cs` que inyecta la cabecera `Content-Security-Policy`.

### 3. Falsificación de Petición en Sitios Cruzados (CSRF)
**Problema mitigado:** Ataques donde un usuario autenticado es engañado para enviar peticiones no deseadas a la aplicación.
**Solución implementada:** Se utilizan de manera nativa los **Tokens Anti-Falsificación (Anti-Forgery Tokens)**.

### 4. Autenticación y Gestión de Sesiones Rota (Y Mejoras de Laboratorio)
**Problema mitigado:** Robo de credenciales, almacenamiento inseguro de contraseñas y exposición de datos en tránsito.
**Solución implementada:** 
- **Hashing Robusto:** Las contraseñas se almacenan con algoritmos robustos (PBKDF2 con HMAC-SHA256).
- **Protección de Datos en Tránsito (HTTP POST vs GET):** Se instruyó sobre la importancia de evitar "Seguridad por oscuridad". Las credenciales nunca viajan en la URL (GET), sino protegidas en el Cuerpo (Body) de las peticiones mediante HTTP POST y encriptadas vía HTTPS.
- **Servicio de Correos Simulados (EmailSender):** Se configuró un servicio para envío de correos (verificables en la consola del servidor) para habilitar confirmaciones de cuenta y Autenticación de 2 Factores (2FA).
- **Personalización de Interfaz Segura:** Se extrajeron y tradujeron al español las pantallas internas de Identity (como "Eliminar Datos Personales", "Contraseña" y "Autenticación 2FA") aplicando el diseño corporativo de Comidasa para evitar interfaces rotas o en inglés que generen desconfianza en el usuario.

### 5. Configuraciones y Cabeceras de Seguridad
En la inicialización del Request Pipeline (`Program.cs`), se agregaron cabeceras HTTP de seguridad:
- `X-Frame-Options: DENY`: Evita ataques de **Clickjacking**.
- `X-Content-Type-Options: nosniff`: Instruye al navegador a no intentar adivinar los tipos MIME.
- `HSTS (HTTP Strict Transport Security)`: Fuerza la comunicación cifrada HTTPS.

## Guía de Despliegue Local
1. Navegar a la carpeta del proyecto `Comidasa`.
2. Restaurar dependencias y ejecutar migraciones si es necesario.
3. Iniciar el servidor con recarga en vivo: `dotnet watch run`
4. Acceder vía navegador (generalmente a través de `http://localhost:5186`).

## Exposición Pública con ngrok (para Presentaciones)

Para presentar la aplicación en vivo desde tu máquina local sin necesidad de desplegarla en un hosting/nube externo, puedes utilizar **ngrok** para crear un túnel seguro temporal.

### Requisitos previos
1. Descargar e instalar [ngrok](https://ngrok.com/).
2. Iniciar sesión y autenticar tu terminal ejecutando el comando de tu cuenta:
   ```bash
   ngrok config add-authtoken <TU_TOKEN>
   ```

### Pasos para exponer el proyecto
1. Asegúrate de que el servidor local de Comidasa esté en ejecución (usualmente en `http://localhost:5186`).
2. En una nueva terminal, abre el túnel indicando el puerto y reescribiendo la cabecera `Host` (requerido para evitar errores `400 Bad Request` en ASP.NET Core):
   ```bash
   ngrok http http://localhost:5186 --host-header="localhost:5186"
   ```
3. Copia la dirección pública generada (de tipo `https://xxxx.ngrok-free.app`).
4. Usa esa dirección en tu presentación para que cualquier persona o dispositivo móvil pueda acceder a tu app en tiempo real.

