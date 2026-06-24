using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Comidasa.Models;
using Comidasa.Data;
using Microsoft.EntityFrameworkCore;

namespace Comidasa.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Obtener productos de la base de datos
        var products = await _context.Products.ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.IdProduct == id);
        if (product == null)
        {
            return NotFound();
        }

        // Obtener productos relacionados
        var relatedProducts = await _context.Products
            .Where(p => p.IdProduct != id)
            .Take(2)
            .ToListAsync();
            
        ViewBag.RelatedProducts = relatedProducts;

        // Cargar reseñas reales con la información del usuario
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
            
        ViewBag.Reviews = reviews;

        return View(product);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Emails()
    {
        var emails = Comidasa.Services.EmailTracker.GetSentEmails();
        return View(emails);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
