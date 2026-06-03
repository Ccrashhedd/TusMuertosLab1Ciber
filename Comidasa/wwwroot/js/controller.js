const app = document.getElementById("app");
const header = document.getElementById("header");
const footer = document.getElementById("footer");

const rutas = {
    "/catalogo": "./Views/catalogo.html",
    "/login": "./Views/login.html",
    "/registro": "./Views/registro.html",
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
        contenedor.innerHTML = "<p class='errorPage'>Error al cargar una sección de la página.</p>";
    }
}

async function cargarVista(ruta) {
    const rutaNormalizada = rutas[ruta] ? ruta : "/catalogo";
    const archivo = rutas[rutaNormalizada];

    try {
        const respuesta = await fetch(archivo);

        if (!respuesta.ok) {
            throw new Error(`No se pudo cargar ${archivo}`);
        }

        const html = await respuesta.text();
        app.innerHTML = html;
        marcarRutaActiva(rutaNormalizada);
        prepararFormularioLogin();
        prepararBusquedaCatalogo();
        prepararFormularioRegistro();
        window.scrollTo({ top: 0, behavior: "smooth" });

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

function marcarRutaActiva(rutaActual) {
    document.querySelectorAll("[data-ruta]").forEach((boton) => {
        const ruta = boton.getAttribute("data-ruta");
        boton.classList.toggle("active", ruta === rutaActual);
    });
}

function prepararFormularioLogin() {
    const formulario = document.getElementById("loginForm");

    if (!formulario) {
        return;
    }

    formulario.addEventListener("submit", function (event) {
        event.preventDefault();
        alert("Formulario validado visualmente. Aquí luego puedes conectar el backend o la base de datos.");
    });
}

function prepararBusquedaCatalogo() {
    const input = document.getElementById("searchInput");
    const boton = document.getElementById("searchButton");
    const tarjetas = document.querySelectorAll(".productoCard");

    if (!input || !boton || tarjetas.length === 0) {
        return;
    }

    const filtrar = () => {
        const texto = input.value.toLowerCase().trim();
        tarjetas.forEach((tarjeta) => {
            const contenido = tarjeta.textContent.toLowerCase();
            tarjeta.style.display = contenido.includes(texto) ? "" : "none";
        });
    };

    boton.addEventListener("click", filtrar);
    input.addEventListener("input", filtrar);
}

function prepararFormularioRegistro() {
    const formulario = document.getElementById("registroForm");

    if (!formulario) {
        return;
    }

    const prepararToggle = (botonId, inputId) => {
        const boton = document.getElementById(botonId);
        const input = document.getElementById(inputId);

        if (!boton || !input) {
            return;
        }

        boton.addEventListener("click", function () {
            const mostrar = input.type === "password";
            input.type = mostrar ? "text" : "password";
            boton.textContent = mostrar ? "Ocultar Contraseña" : "Mostrar Contraseña";
        });
    };

    prepararToggle("toggleContrasen", "contrasen");
    prepararToggle("toggleConfirmarContrasen", "confirmarContrasen");

    formulario.addEventListener("submit", function (event) {
        event.preventDefault();

        const contrasen = document.getElementById("contrasen").value;
        const confirmarContrasen = document.getElementById("confirmarContrasen").value;

        if (contrasen !== confirmarContrasen) {
            alert("Las contraseñas no coinciden.");
            return;
        }

        alert("Registro validado visualmente. Luego puedes conectar este formulario al backend.");
    });
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
    if (!app || !header || !footer) {
        return;
    }

    await cargarParcial(header, "./Partials/header.html");
    await cargarParcial(footer, "./Partials/footer.html");

    const rutaInicial = location.hash.replace("#", "") || "/catalogo";
    cargarVista(rutaInicial);
});
