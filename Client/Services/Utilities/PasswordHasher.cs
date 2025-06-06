using System;
using System.Security.Cryptography;

namespace CSE443_Project.Services.Utilities
{
    public static class PasswordHasher
    {
        // Generate a salt and hash the password
        public static string HashPassword(string password)
        {
            // Generate a salt (work factor of 12 is recommended)
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        // Verify a password against a hash
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}