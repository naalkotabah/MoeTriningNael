// Moe.Core/Helpers/PasswordHelper.cs

using System.Security.Cryptography;
using System.Text;

namespace Moe.Core.Helpers
{
    public static class PasswordHelper
    {
      
        public static (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
        {
            using (var hmac = new HMACSHA512())
            {
       
                var passwordSalt = hmac.Key;

             
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                return (passwordHash, passwordSalt); 
            }
        }
    }
}
