using System.Security.Cryptography;

namespace Comprehension.Services
{
    public class TokenService
    {
        // 256 bits = 32 bytes
        private const int TokenSizeInBytes = 32;

        public string GenerateToken()
        {
            // Generar bytes aleatorios criptográficamente seguros
            byte[] tokenBytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);
            // Convertir a Base64 para que sea una cadena de texto
            return Convert.ToBase64String(tokenBytes);
        }
    }
}