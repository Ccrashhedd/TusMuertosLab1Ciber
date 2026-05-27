using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Comidasa.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:Username"];
            var smtpPass = _configuration["Smtp:Password"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                _logger.LogWarning("Configuración SMTP incompleta. No se pudo enviar el correo a {Email}", email);
                return;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, "Seguridad Comidasa")
            };
            
            mailMessage.Subject = subject;

            // Remove HTML tags for the plain text version
            string plainText = System.Text.RegularExpressions.Regex.Replace(htmlMessage, "<.*?>", string.Empty);
            
            // Build a more complete HTML email structure
            string fullHtml = $@"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <title>{subject}</title>
            </head>
            <body style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #f3f4f6; color: #1f2937; line-height: 1.6; padding: 40px 20px; margin: 0;'>
                <div style='max-width: 550px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);'>
                    <div style='background-color: #2563eb; padding: 30px; text-align: center;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 700; letter-spacing: -0.5px;'>Comidasa</h1>
                    </div>
                    <div style='padding: 40px 30px;'>
                        <h2 style='margin-top: 0; color: #111827; font-size: 20px; font-weight: 600;'>Código de Verificación</h2>
                        <p style='color: #4b5563; font-size: 16px; margin-bottom: 30px;'>
                            Hola, hemos recibido una solicitud para iniciar sesión en tu cuenta. Usa el siguiente código de seguridad:
                        </p>
                        
                        <div style='background-color: #f8fafc; border: 1px dashed #cbd5e1; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
                            <div style='font-size: 36px; font-weight: 800; letter-spacing: 6px; color: #0f172a;'>
                                {htmlMessage.Replace("Su código de seguridad es: <b>", "").Replace("</b>. Introdúzcalo en la aplicación para iniciar sesión.", "").Replace("Su nuevo código de seguridad es: <b>", "")}
                            </div>
                        </div>

                        <p style='color: #64748b; font-size: 14px; margin-bottom: 0;'>
                            Este código expirará en 30 segundos. Si no solicitaste este código, puedes ignorar este correo de forma segura.
                        </p>
                    </div>
                    <div style='background-color: #f8fafc; padding: 20px 30px; text-align: center; border-top: 1px solid #e2e8f0;'>
                        <p style='margin: 0; font-size: 12px; color: #94a3b8;'>
                            &copy; {DateTime.Now.Year} Comidasa. Todos los derechos reservados.<br>
                            Este es un mensaje automático, por favor no responda.
                        </p>
                    </div>
                </div>
            </body>
            </html>";

            // Add alternate views
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainText, null, "text/plain"));
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(fullHtml, null, "text/html"));

            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Correo enviado exitosamente a {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo a {Email}", email);
                throw;
            }
        }
    }
}
