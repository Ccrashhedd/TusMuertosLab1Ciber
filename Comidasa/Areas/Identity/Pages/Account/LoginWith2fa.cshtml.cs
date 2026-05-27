using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Comidasa.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<LoginWith2faModel> _logger;
        private readonly IMemoryCache _cache;

        public LoginWith2faModel(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            ILogger<LoginWith2faModel> logger,
            IMemoryCache cache)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _cache = cache;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Código de seguridad")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Recordar esta máquina")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException($"Incapaz de cargar al usuario de dos factores.");
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Incapaz de cargar al usuario de dos factores.");
            }

            // Verificar si el código ha expirado en la caché (30 segundos)
            if (!_cache.TryGetValue($"2FA_Time_{user.Id}", out _))
            {
                ModelState.AddModelError(string.Empty, "El código ha expirado después de 30 segundos. Por favor, solicita uno nuevo.");
                return Page();
            }

            // Clean up the code.
            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            // Use "Email" provider instead of Authenticator
            var result = await _signInManager.TwoFactorSignInAsync("Email", authenticatorCode, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("El usuario con ID '{UserId}' ha iniciado sesión con 2FA.", user.Id);
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("El usuario con ID '{UserId}' está bloqueado.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Código de autenticación de 2 factores inválido introducido para el usuario con ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Código de autenticación inválido.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResendAsync(bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Incapaz de cargar al usuario de dos factores.");
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            
            // Reiniciar el tiempo de expiración (30 segundos)
            _cache.Set($"2FA_Time_{user.Id}", DateTime.UtcNow, TimeSpan.FromSeconds(30));

            await _emailSender.SendEmailAsync(
                user.Email,
                "Código de Autenticación de Dos Factores (Reenviado)",
                $"Su nuevo código de seguridad es: <b>{code}</b>. Introdúzcalo en la aplicación para iniciar sesión.");

            ModelState.AddModelError(string.Empty, "Se ha enviado un nuevo código a su correo electrónico.");
            
            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }
    }
}
