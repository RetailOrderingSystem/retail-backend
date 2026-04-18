// Services/EmailService.cs
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RetailAPI.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otp);
        Task SendVerificationEmailAsync(string email, string token);
        Task SendLoginAlertAsync(string email);
    }

    public class EmailService : IEmailService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendOtpEmailAsync(string email, string otp)
        {
            var subject = "Your Login OTP - Retail API";
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #eee;border-radius:8px;'>
                    <h2 style='color:#2e7d32;'>Login Verification</h2>
                    <p>Your One-Time Password (OTP) is:</p>
                    <h1 style='background:#f5f5f5;padding:15px;text-align:center;letter-spacing:8px;color:#2e7d32;border-radius:5px;'>{otp}</h1>
                    <p>This OTP is valid for <b>5 minutes</b>.</p>
                    <p style='color:#888;font-size:12px;'>If you did not request this, please ignore this email.</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendVerificationEmailAsync(string email, string token)
        {
            var subject = "Verify Your Email - Retail API";
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #eee;border-radius:8px;'>
                    <h2 style='color:#1565c0;'>Welcome to Retail API!</h2>
                    <p>Please verify your email using the token below:</p>
                    <h1 style='background:#f5f5f5;padding:15px;text-align:center;letter-spacing:8px;color:#1565c0;border-radius:5px;'>{token}</h1>
                    <p>This token is valid for <b>24 hours</b>.</p>
                    <p style='color:#888;font-size:12px;'>If you did not create an account, please ignore this email.</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendLoginAlertAsync(string email)
        {
            var subject = "New Login Alert - Retail API";
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #eee;border-radius:8px;'>
                    <h2 style='color:#ef6c00;'>Login Notification</h2>
                    <p>Your account was just logged in at <b>{DateTime.UtcNow:u}</b>.</p>
                    <p style='color:#888;font-size:12px;'>If this wasn't you, please reset your password immediately.</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Try Mailtrap / HTTP API first (existing behaviour)
                var apiToken = _configuration["EmailSettings:ApiToken"];
                var inboxId = _configuration["EmailSettings:InboxId"];
                var apiUrl = _configuration["EmailSettings:ApiUrl"] ?? "https://sandbox.api.mailtrap.io/api/send";
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? _configuration["EmailSettings:FromEmail"] ?? "noreply@retailapi.com";
                var senderName = _configuration["EmailSettings:SenderName"] ?? _configuration["EmailSettings:FromName"] ?? "Retail API";

                if (!string.IsNullOrWhiteSpace(apiToken) && !string.IsNullOrWhiteSpace(inboxId))
                {
                    var fullUrl = $"{apiUrl}/{inboxId}";

                    var payload = new
                    {
                        from = new { email = senderEmail, name = senderName },
                        to = new[] { new { email = toEmail } },
                        subject,
                        html = body,
                        category = "Retail API"
                    };

                    var json = JsonSerializer.Serialize(payload);

                    using var client = _httpClientFactory.CreateClient();
                    client.DefaultRequestHeaders.Remove("Api-Token");
                    client.DefaultRequestHeaders.Add("Api-Token", apiToken);
                    client.DefaultRequestHeaders.Remove("Accept");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    using var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _logger.LogInformation("📧 Sending email via Mailtrap API to {Email}", toEmail);

                    var response = await client.PostAsync(fullUrl, content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("✅ Email sent successfully to {Email} via Mailtrap API", toEmail);
                    }
                    else
                    {
                        _logger.LogError("❌ Failed to send email via Mailtrap API. Status: {Status}, Response: {Response}",
                            response.StatusCode, responseBody);
                    }

                    return;
                }

                // If Mailtrap not configured, fall back to SMTP using appsettings EmailSettings
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                if (string.IsNullOrWhiteSpace(smtpHost))
                {
                    _logger.LogError("Email sending skipped: neither Mailtrap API nor SMTP is configured (check EmailSettings in configuration).");
                    return;
                }

                var smtpPortValue = _configuration["EmailSettings:SmtpPort"];
                int smtpPort = 25;
                if (!string.IsNullOrWhiteSpace(smtpPortValue) && int.TryParse(smtpPortValue, out var parsedPort))
                    smtpPort = parsedPort;

                var enableSslValue = _configuration["EmailSettings:EnableSsl"];
                bool enableSsl = true;
                if (!string.IsNullOrWhiteSpace(enableSslValue) && bool.TryParse(enableSslValue, out var parsedSsl))
                    enableSsl = parsedSsl;

                var senderPassword = _configuration["EmailSettings:SenderPassword"];
                // support FromEmail/FromName as alternate keys
                senderEmail = _configuration["EmailSettings:FromEmail"] ?? senderEmail;
                senderName = _configuration["EmailSettings:FromName"] ?? senderName;

                using var mail = new MailMessage();
                mail.From = new MailAddress(senderEmail, senderName);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl
                };

                if (!string.IsNullOrWhiteSpace(senderPassword))
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

                _logger.LogInformation("📧 Sending email via SMTP to {Email} using host {Host}:{Port}", toEmail, smtpHost, smtpPort);
                await smtpClient.SendMailAsync(mail);
                _logger.LogInformation("✅ Email sent successfully to {Email} via SMTP", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending email to {Email}", toEmail);
            }
        }
    }
}