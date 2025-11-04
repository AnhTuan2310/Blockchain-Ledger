using Application.IServices;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.ExternalServices {
    public class SimpleHashingService : IHashingService {
        public string ComputeHash(string input) {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
