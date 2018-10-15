using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Myvas.AspNetCore.TencentCos
{
    internal static class HashHelper
    {
        public static string HmacSha1(string key, string content)
        {
            var buff = new HMACSHA1(Encoding.UTF8.GetBytes(key)).ComputeHash(Encoding.UTF8.GetBytes(content));
            return string.Concat(buff.Select(k => k.ToString("x2")));
        }

        public static string Sha1(string content)
        {
            var buff = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(content));
            return string.Concat(buff.Select(k => k.ToString("x2")));
        }
    }
}
