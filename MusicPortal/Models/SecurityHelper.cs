using System.Security.Cryptography;

namespace MusicPortal.Models
{
    public class SecurityHelper
    {
        public static string GenerateSalt(int size)
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[size];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static string HashPassword(string password, string salt, int iterations, int hashSize)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations);
            return Convert.ToBase64String(pbkdf2.GetBytes(hashSize));
        }
    }
}
