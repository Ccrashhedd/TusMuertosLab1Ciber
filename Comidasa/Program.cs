using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Comidasa.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Registrar el servicio de envío de correos
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

app.UseHttpsRedirection();

// Configuraciones de Seguridad para Cumplimiento
app.Use(async (context, next) =>
{
    // Content Security Policy (Prevención de XSS)
    // Se agregó https://images.unsplash.com a img-src para permitir cargar las imágenes del menú
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
