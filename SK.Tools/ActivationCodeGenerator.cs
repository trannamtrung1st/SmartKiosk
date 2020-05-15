using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SK.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Tools
{
    public class ActivationCodeGenerator
    {
        public string GenerateCode(string username)
        {
            var bytes = Encoding.ASCII.GetBytes(ActivationCodeSecrect.SECRET);
            var code = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                username,
                bytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return code;
        }

        public void Start()
        {
            Console.Write("Input username: ");
            var username = Console.ReadLine();
            var code = GenerateCode(username);
            Console.WriteLine(code);
        }
    }
}
