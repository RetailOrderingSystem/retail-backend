// Services/EmailService.cs
using System.Text;
using System.Text.Json;

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
                var apiToken = _configuration["EmailSettings:ApiToken"];
                var inboxId = _configuration["EmailSettings:InboxId"];
                var apiUrl = _configuration["EmailSettings:ApiUrl"] ?? "https://sandbox.api.mailtrap.io/api/send";
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "noreply@retailapi.com";
                var senderName = _configuration["EmailSettings:SenderName"] ?? "Retail API";

                if (string.IsNullOrWhiteSpace(apiToken) || string.IsNullOrWhiteSpace(inboxId))
                {
                    _logger.LogError("Mailtrap API token or Inbox ID is not configured.");
                    return;
                }

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
                client.DefaultRequestHeaders.Add("Api-Token", apiToken);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("📧 Sending email via Mailtrap API to {Email}", toEmail);

                var response = await client.PostAsync(fullUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✅ Email sent successfully to {Email}", toEmail);
                }
                else
                {
                    _logger.LogError("❌ Failed to send email. Status: {Status}, Response: {Response}",
                        response.StatusCode, responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending email to {Email}", toEmail);
            }
        }
    }
}