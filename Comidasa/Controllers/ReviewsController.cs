using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comidasa.Data;
using Comidasa.Models;

namespace Comidasa.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ReviewsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review, IFormFile? documentFile)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            review.UserId = userId;
            review.CreatedAt = DateTime.Now;

            // Handle file upload
            if (documentFile != null && documentFile.Length > 0)
            {
                try
                {
                    // Create directory if not exists
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads", "Reviews");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(documentFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await documentFile.CopyToAsync(fileStream);
                    }

                    review.DocumentPath = "/Uploads/Reviews/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al subir el archivo: " + ex.Message);
                }
            }

            // Remove navigation properties from validation so it doesn't fail on model state
            ModelState.Remove("User");
            ModelState.Remove("Product");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Home", new { id = review.ProductId });
            }

            // If we got here, something failed, redirect back to details
            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string comment, int rating, string documento, IFormFile? documentFile, string? keepCurrentFile)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (review.UserId != userId)
            {
                return Forbid();
            }

            review.Comment = comment;
            review.Rating = rating;
            review.Documento = documento ?? string.Empty;

            // Handle file upload on Edit
            if (documentFile != null && documentFile.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads", "Reviews");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(documentFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await documentFile.CopyToAsync(fileStream);
                    }

                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(review.DocumentPath))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, review.DocumentPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    review.DocumentPath = "/Uploads/Reviews/" + uniqueFileName;
                }
                catch (Exception)
                {
                    // Ignore or log file deletion errors
                }
            }
            else if (keepCurrentFile == "false")
            {
                // Delete file if user requested removal
                if (!string.IsNullOrEmpty(review.DocumentPath))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, review.DocumentPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    review.DocumentPath = null;
                }
            }

            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Redirect back to referring page (could be product details or user profile)
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (review.UserId != userId)
            {
                return Forbid();
            }

            // Delete associated file if exists
            if (!string.IsNullOrEmpty(review.DocumentPath))
            {
                try
                {
                    var filePath = Path.Combine(_environment.WebRootPath, review.DocumentPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch (Exception)
                {
                    // Ignore file deletion errors
                }
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }
    }
}
