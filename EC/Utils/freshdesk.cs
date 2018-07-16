using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EC.Utils
{
    public class FreshDesk
    {
        public static string GetSsoUrl(HttpServerUtilityBase Server, string baseUrl, string secret, string name, string email)
        {
            var timems = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
            return String.Format("{0}/login/sso?name={1}&email={2}&timestamp={3}&hash={4}",
                baseUrl, Server.UrlEncode(name), Server.UrlEncode(email), timems, GetHash(secret, name, email, timems));
        }

        private static string GetHash(string secret, string name, string email, string timems)
        {
            var input = name + secret + email + timems;
            var keybytes = Encoding.UTF8.GetBytes(secret);
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var crypto = new HMACMD5(keybytes);
            var hash = crypto.ComputeHash(inputBytes);
            return hash.Select(b => b.ToString("x2"))
                .Aggregate(new StringBuilder(),
                    (current, next) => current.Append(next),
                    current => current.ToString());
        }
    }
}