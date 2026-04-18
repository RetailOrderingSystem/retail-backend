namespace RetailAPI.Helpers
{
    // NOTE: Simple plain-text password handler.
    // Replace with BCrypt or Identity in production.
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Simple: return as-is (plain text)
            return password;
        }

        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return inputPassword == storedPassword;
        }
    }
}

