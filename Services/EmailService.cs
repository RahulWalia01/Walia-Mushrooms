using System.Net;
using System.Net.Mail;
using MushroomApi.Models;

namespace MushroomApi.Services
{
    public interface IEmailService
    {
        Task SendInquiryNotificationAsync(Inquiry inquiry);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendInquiryNotificationAsync(Inquiry inquiry)
        {
            // Read settings from appsettings.json
            var smtpHost = _config["Email:SmtpHost"]!;
            var smtpPort = int.Parse(_config["Email:SmtpPort"]!);
            var smtpUser = _config["Email:SmtpUser"]!;     // Your Gmail address
            var smtpPassword = _config["Email:SmtpPassword"]!; // Your Gmail App Password
            var toEmail = _config["Email:ToEmail"]!;      // Where you want to receive inquiries

            // Build the email body
            var body = $"""
                <h2 style="color:#2D5016;">New Inquiry — Walia Mushrooms</h2>
                <table style="border-collapse:collapse;width:100%;font-family:sans-serif;">
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Name</td>
                        <td style="padding:8px;">{inquiry.FullName}</td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Company</td>
                        <td style="padding:8px;">{inquiry.Company}</td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Email</td>
                        <td style="padding:8px;"><a href="mailto:{inquiry.Email}">{inquiry.Email}</a></td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Phone</td>
                        <td style="padding:8px;">{inquiry.Phone}</td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Order Type</td>
                        <td style="padding:8px;">{inquiry.OrderType}</td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Product</td>
                        <td style="padding:8px;">{inquiry.Product}</td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Quantity</td>
                        <td style="padding:8px;">{inquiry.Quantity}</td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Message</td>
                        <td style="padding:8px;">{inquiry.Message}</td></tr>
                    <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Submitted At</td>
                        <td style="padding:8px;">{inquiry.SubmittedAt:dd MMM yyyy, hh:mm tt} UTC</td></tr>
                </table>
                <br/>
                <a href="http://localhost:5000/admin.html" 
                   style="background:#2D5016;color:white;padding:10px 20px;
                          border-radius:20px;text-decoration:none;font-family:sans-serif;">
                    View All Inquiries →
                </a>
                """;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "Walia Mushrooms Website"),
                Subject = $"New Inquiry from {inquiry.FullName} — {inquiry.Product}",
                Body = body,
                IsBodyHtml = true,
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
    }
}
