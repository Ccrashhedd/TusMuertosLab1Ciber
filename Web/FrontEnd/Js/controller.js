const app = document.getElementById("app");
const header = document.getElementById("header");
const footer = document.getElementById("footer");

const rutas = {
    "/catalogo": "./Views/catalogo.html",
    "/login": "./Views/login.html",
    "/producto": "./Views/producto.html"
};

async function cargarParcial(contenedor, archivo) {
    try {
        const respuesta = await fetch(archivo);

        if (!respuesta.ok) {
            throw new Error(`No se pudo cargar ${archivo}`);
        }

        const html = await respuesta.text();
        contenedor.innerHTML = html;

    } catch (error) {
        console.error(error);
        contenedor.innerHTML = "<p>Error al cargar una sección de la página.</p>";
    }
}

async function cargarVista(ruta) {
    const archivo = rutas[ruta] || rutas["/catalogo"];

    try {
        const respuesta = await fetch(archivo);

        if (!respuesta.ok) {
            throw new Error(`No se pudo cargar ${archivo}`);
        }

        const html = await respuesta.text();
        app.innerHTML = html;

    } catch (error) {
        console.error(error);
        app.innerHTML = `
            <section class="errorPage">
                <h2>Error</h2>
                <p>No se pudo cargar la vista solicitada.</p>
            </section>
        `;
    }
}

function navegar(ruta) {
    location.hash = ruta;
}

document.addEventListener("click", function (event) {
    const boton = event.target.closest("[data-ruta]");

    if (boton) {
        const ruta = boton.getAttribute("data-ruta");
        navegar(ruta);
    }
});

window.addEventListener("hashchange", function () {
    const ruta = location.hash.replace("#", "") || "/catalogo";
    cargarVista(ruta);
});

document.addEventListener("DOMContentLoaded", async function () {
    await cargarParcial(header, "./Partials/header.html");
    await cargarParcial(footer, "./Partials/footer.html");

    const rutaInicial = location.hash.replace("#", "") || "/catalogo";
    cargarVista(rutaInicial);
});