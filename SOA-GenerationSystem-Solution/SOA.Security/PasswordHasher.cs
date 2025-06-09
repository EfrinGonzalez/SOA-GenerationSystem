using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SOA.Interfaces;

namespace SOA.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            // For demo: use a simple hash (replace in production)
            byte[] salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        public bool Verify(string password, string hash)
        {
            // For now: this just simulates always valid (replace with real logic)
            return true;
        }
    }
}
