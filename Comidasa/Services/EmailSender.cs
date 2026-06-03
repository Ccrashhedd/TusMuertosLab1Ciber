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
            
            // Extract the 6-digit verification code using robust regex
            string code = "";
            var match = System.Text.RegularExpressions.Regex.Match(htmlMessage, @"<b>(\d+)</b>");
            if (match.Success)
            {
                code = match.Groups[1].Value;
            }
            else
            {
                var matchBold = System.Text.RegularExpressions.Regex.Match(htmlMessage, @"<b>(.*?)</b>");
                code = matchBold.Success ? matchBold.Groups[1].Value : htmlMessage;
            }

            // Clean the instruction message to show beneath the code
            string cleanInstruction = htmlMessage;
            if (match.Success)
            {
                cleanInstruction = htmlMessage
                    .Replace($"<b>{code}</b>", "")
                    .Replace("Su código de seguridad es: ", "")
                    .Replace("Su nuevo código de seguridad es: ", "")
                    .Trim();
                
                // Capitalize first letter
                if (cleanInstruction.Length > 0)
                {
                    cleanInstruction = char.ToUpper(cleanInstruction[0]) + cleanInstruction.Substring(1);
                }
            }

            // Determine context text to show in the description
            string actionDescription = "validar tu cuenta";
            if (htmlMessage.Contains("iniciar sesión"))
            {
                actionDescription = "iniciar sesión en tu cuenta";
            }
            else if (htmlMessage.Contains("verificar su cuenta") || htmlMessage.Contains("verificar tu cuenta"))
            {
                actionDescription = "verificar tu cuenta de correo";
            }

            string cleanInstructionHtml = !string.IsNullOrEmpty(cleanInstruction)
                ? $"<p style='color: #4b5563; font-size: 14px; text-align: center; margin-bottom: 24px; line-height: 1.5; font-family: sans-serif;'>{cleanInstruction}</p>"
                : "";

            // Build a responsive HTML email structure using brand colors (#a8320e)
            string fullHtml = $@"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>{subject}</title>
            </head>
            <body style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #f3f4f6; color: #1f2937; line-height: 1.6; padding: 20px; margin: 0;'>
                <div style='max-width: 450px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);'>
                    <div style='background-color: #a8320e; padding: 24px; text-align: center;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 700; letter-spacing: -0.5px; font-family: ""Plus Jakarta Sans"", sans-serif;'>Comidasa</h1>
                    </div>
                    <div style='padding: 30px 24px;'>
                        <h2 style='margin-top: 0; color: #111827; font-size: 18px; font-weight: 600; text-align: center;'>Código de Seguridad</h2>
                        
                        <p style='color: #4b5563; font-size: 15px; text-align: center; margin-bottom: 24px; margin-top: 10px;'>
                            Hola, hemos recibido una solicitud para {actionDescription}. Usa el siguiente código de seguridad para continuar:
                        </p>
                        
                        <div style='background-color: #fdf5f3; border: 1px dashed #d18d77; border-radius: 8px; padding: 18px 10px; text-align: center; margin-bottom: 24px;'>
                            <div style='font-size: 32px; font-weight: 800; letter-spacing: 6px; color: #a8320e; font-family: monospace; display: inline-block;'>
                                {code}
                            </div>
                        </div>

                        {cleanInstructionHtml}

                        <p style='color: #94a3b8; font-size: 12px; text-align: center; margin-bottom: 0;'>
                            Este código es temporal. Si no solicitaste este código, puedes ignorar este correo de forma segura.
                        </p>
                    </div>
                    <div style='background-color: #f8fafc; padding: 16px 24px; text-align: center; border-top: 1px solid #e2e8f0;'>
                        <p style='margin: 0; font-size: 11px; color: #94a3b8;'>
                            &copy; {DateTime.Now.Year} Comidasa. Todos los derechos reservados.<br>
                            Este es un mensaje automático, por favor no lo responda.
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
