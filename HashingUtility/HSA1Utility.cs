using System.Security.Cryptography;
using System.Text;
using System;
using WebApplication1.Cache;
using WebApplication1.Models;

namespace WebApplication1.HashingUtility
{
    public static class HSA1Utility
    {
        public static string Hash(string data)
        {
            string hash = null;

            using (SHA1 sha1Hash = SHA1.Create())
            {

                byte[] jsonAsBytes = Encoding.UTF8.GetBytes(data);
                byte[] hashBytes = sha1Hash.ComputeHash(jsonAsBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);                
            }

            return hash;
        }
    }
}
