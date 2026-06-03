using System;
using System.Net;
using System.Net.Mail;

namespace TestSmtp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var smtpHost = "smtp.gmail.com";
                var smtpPort = 587;
                var smtpUser = "comidasa78@gmail.com";
                var smtpPass = "mipgoqbuvhjmefpf";

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, "Seguridad Comidasa"),
                    Subject = "Test Email",
                    Body = "This is a test email."
                };
                mailMessage.To.Add(smtpUser); // Send to self

                Console.WriteLine("Sending email...");
                client.Send(mailMessage);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Error: " + ex.InnerException.Message);
                }
            }
        }
    }
}
