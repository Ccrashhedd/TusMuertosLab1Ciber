# Cambios de diseño aplicados a Comidasa

Se rediseñaron las páginas principales del proyecto usando Bootstrap local y CSS personalizado.

## Archivos modificados

- `Views/Shared/_Layout.cshtml`
- `Views/Shared/_LoginPartial.cshtml`
- `Views/Home/Index.cshtml`
- `Views/Home/Privacy.cshtml`
- `Areas/Identity/Pages/Account/Login.cshtml`
- `Areas/Identity/Pages/Account/Register.cshtml`
- `wwwroot/css/site.css`
- `wwwroot/js/site.js`

## Paleta usada

- Primary: `#F24C05`
- Secondary: `#2D5A27`
- Tertiary: `#F2B705`
- Neutral: `#262626`

## Notas

- El diseño mantiene las rutas y formularios de Identity para no romper login, registro y cierre de sesión.
- Bootstrap se carga desde `wwwroot/lib/bootstrap`, por lo que no depende de CDN para el layout principal.
- Las fuentes e iconos usan Google Fonts y Material Symbols. Si no hay Internet, el sitio seguirá funcionando, pero puede cambiar la fuente o no mostrarse el icono.
- No se pudo ejecutar `dotnet build` en este entorno porque no está instalado el SDK de .NET.

## Frontend estático también actualizado

También se actualizó la carpeta `Web/FrontEnd` para que las vistas HTML usen la misma estética:

- `Web/FrontEnd/index.html`
- `Web/FrontEnd/Partials/header.html`
- `Web/FrontEnd/Partials/footer.html`
- `Web/FrontEnd/Views/catalogo.html`
- `Web/FrontEnd/Views/producto.html`
- `Web/FrontEnd/Views/login.html`
- `Web/FrontEnd/Js/controller.js`
- `Web/Styles/main.css`
- `Web/Styles/variables.css`
- `Web/Styles/catalogo.css`
