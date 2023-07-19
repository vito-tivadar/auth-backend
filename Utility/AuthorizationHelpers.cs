using System.Security.Cryptography;
using System.Text;

namespace auth_backend.Utility;

// Helper class for Authorization (register, login)
public static class AuthorizationHelpers
{
    public static void HashPassword(
        string password,
        out byte[] newPasswordHash,
        out byte[] passwordSalt
    )
    {
        using (var sha256 = new HMACSHA256())
        {
            passwordSalt = sha256.Key;
            newPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    public static bool VerifyPasswordHash(
        string password,
        byte[] storedPasswordHash,
        byte[] storedPasswordSalt
    )
    {
        using (var sha256 = new HMACSHA256(storedPasswordSalt))
        {
            var newInputPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return newInputPasswordHash.SequenceEqual(storedPasswordHash);
        }
    }
};
