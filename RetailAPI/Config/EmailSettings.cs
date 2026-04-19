namespace RetailAPI.Config
{
    public class EmailSettings
    {
        // Mail API (Mailtrap)
        public string ApiToken { get; set; } = string.Empty;
        public string InboxId { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;

        // SMTP
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}

