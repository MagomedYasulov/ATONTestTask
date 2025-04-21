using ATONTestTask.Abstractions;
using BCrypt.Net;
using System.Diagnostics.Eventing.Reader;

namespace ATONTestTask.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA384);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(providedPassword, hashedPassword, HashType.SHA384);
        }
    }
}
