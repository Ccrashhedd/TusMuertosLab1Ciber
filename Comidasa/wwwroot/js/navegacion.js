document.addEventListener('click', function (event) {
    const boton = event.target.closest('[data-ruta]');
    if (!boton) return;

    const ruta = boton.getAttribute('data-ruta');
    const estaEnViews = window.location.pathname.includes('/Views/');

    if (ruta === '/producto') {
        window.location.href = estaEnViews ? './producto.html' : './Views/producto.html';
    }

    if (ruta === '/registro') {
        window.location.href = estaEnViews ? './registro.html' : './Views/registro.html';
    }

    if (ruta === '/login') {
        window.location.href = estaEnViews ? './login.html' : './Views/login.html';
    }

    if (ruta === '/catalogo') {
        window.location.href = estaEnViews ? './catalogo.html' : './Views/catalogo.html';
    }

    if (ruta === '/inicio') {
        window.location.href = estaEnViews ? '../index.html' : './index.html';
    }
});

const searchInput = document.getElementById('searchInput');
const searchButton = document.getElementById('searchButton');

function filtrarProductos() {
    const texto = (searchInput?.value || '').toLowerCase().trim();

    document.querySelectorAll('.productoCard').forEach(function (card) {
        const contenido = card.textContent.toLowerCase();
        card.style.display = contenido.includes(texto) ? '' : 'none';
    });
}

searchButton?.addEventListener('click', filtrarProductos);
searchInput?.addEventListener('input', filtrarProductos);

function prepararToggle(buttonId, inputId) {
    const boton = document.getElementById(buttonId);
    const input = document.getElementById(inputId);

    boton?.addEventListener('click', function () {
        const mostrar = input.type === 'password';
        input.type = mostrar ? 'text' : 'password';
        boton.textContent = mostrar ? 'Ocultar Contraseña' : 'Mostrar Contraseña';
    });
}

prepararToggle('toggleContrasen', 'contrasen');
prepararToggle('toggleConfirmarContrasen', 'confirmarContrasen');

document.getElementById('registroForm')?.addEventListener('submit', function (event) {
    event.preventDefault();

    const pass = document.getElementById('contrasen').value;
    const confirmar = document.getElementById('confirmarContrasen').value;

    if (pass !== confirmar) {
        alert('Las contraseñas no coinciden.');
        return;
    }

    alert('Registro validado visualmente. Luego puedes conectar este formulario al backend.');
});

document.getElementById('loginForm')?.addEventListener('submit', function (event) {
    event.preventDefault();
    alert('Formulario validado visualmente. Aquí luego puedes conectar el backend o la base de datos.');
});
