using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Comidasa.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddMemoryCache();

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
    
    // Configurar bloqueo (lockout) por intentos fallidos
    options.Lockout.MaxFailedAccessAttempts = 4; // Limitar a 4 intentos
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Tiempo de bloqueo
    options.Lockout.AllowedForNewUsers = true; // Aplicar a todos los usuarios
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Registrar el servicio de envÃ­o de correos
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, Comidasa.Services.EmailSender>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await Comidasa.Data.DbSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Configuraciones de Seguridad para Cumplimiento
app.Use(async (context, next) =>
{
    // Content Security Policy (PrevenciÃ³n de XSS)
    // Se agregÃ³ https://images.unsplash.com a img-src para permitir cargar las imÃ¡genes del menÃº
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' https://cdn.tailwindcss.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com; img-src 'self' data: https://images.unsplash.com https://lh3.googleusercontent.com; frame-ancestors 'none';");
    
    // Evitar que el navegador intente adivinar el tipo de contenido (MIME-sniffing)
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    
    // Evitar Clickjacking
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    await next();
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
