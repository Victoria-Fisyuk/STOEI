using System;
using System.Security.Cryptography;
using System.Text;

namespace TAiMStore.HtmlHelpers
{
    public static class HashHelper
    {
        /// <summary>
        /// Метод кодирующий пароль md5
        /// </summary>
        /// <param name="pass">пароль</param>
        /// <returns>хэш пароля </returns>
        public static string HashPassword(string pass)
        {
            // Преобразуем битовое значение пароля 
            var passTextBytes = Encoding.UTF8.GetBytes(pass);

            // хешируем пароль и соль в MD5
            var hash = new SHA1Managed();
            var hashBytes = hash.ComputeHash(passTextBytes);

            // хешируем смешанный пароь с помощью SHA512
            var hashSha1 = new SHA512Managed();
            var hashWithSaltCode = hashSha1.ComputeHash(hashBytes);
            var hashValue = Convert.ToBase64String(hashBytes);

            return hashValue;
        }
    }
}
