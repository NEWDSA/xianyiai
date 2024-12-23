using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace XianYu.API.Infrastructure.Auth;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int HashSize = 256 / 8;
    private const int Iterations = 10000;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        // 生成随机盐值
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // 使用 PBKDF2 生成哈希
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: HashSize
        );

        // 组合盐值和哈希
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // 确保生成的Base64字符串是有效的
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            // 尝试解码Base64字符串
            byte[] hashBytes;
            try
            {
                hashBytes = Convert.FromBase64String(hashedPassword);
            }
            catch (FormatException)
            {
                // 如果不是有效的Base64字符串，返回false
                return false;
            }

            // 验证哈希长度
            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            // 提取盐值
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // 使用相同的参数生成哈希
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: HashSize
            );

            // 比较哈希值
            var result = true;
            for (int i = 0; i < HashSize; i++)
            {
                result &= hashBytes[i + SaltSize] == hash[i];
            }

            return result;
        }
        catch (Exception)
        {
            // 如果发生任何其他错误，返回false
            return false;
        }
    }
} 