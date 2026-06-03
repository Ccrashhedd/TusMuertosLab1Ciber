using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comidasa.Data;
using Comidasa.Models;

namespace Comidasa.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            var favorites = await _context.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return View(favorites);
        }

        // POST: Favorites/ToggleFavorite
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Debe iniciar sesión" });
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            bool isFavoriteNow;

            if (favorite != null)
            {
                // Already a favorite, remove it
                _context.Favorites.Remove(favorite);
                isFavoriteNow = false;
            }
            else
            {
                // Not a favorite, add it
                favorite = new Favorite
                {
                    UserId = userId,
                    ProductId = productId
                };
                _context.Favorites.Add(favorite);
                isFavoriteNow = true;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, isFavorite = isFavoriteNow });
        }
    }
}
