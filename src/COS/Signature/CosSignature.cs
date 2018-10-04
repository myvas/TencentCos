using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace AspNetCore.TencentCos
{
    public class CosSignature
    {
        public const string SignAlgorithm = "sha1";

        public string SecretId { get; set; }
        public string SecretKey { get; set; }
        public HttpRequestMessage HttpRequestMessage { get; set; }

        public CosSignature(HttpRequestMessage req, string secretId, string secretKey)
        {
            HttpRequestMessage = req ?? throw new ArgumentNullException(nameof(req));
            SecretId = secretId;
            SecretKey = secretKey;
        }

        private string CalculateSignature(CosTimestamp timestamp)
        {
            var secretKey = SecretKey;

            var signKey = HashHelper.HmacSha1(secretKey, timestamp.ToString());
            var httpString = HttpRequestMessage.ToHttpString();
            var hashedHttpString = HashHelper.Sha1(httpString);

            var stringToSign = $"{SignAlgorithm}\n{timestamp}\n{hashedHttpString}\n";
            var result = HashHelper.HmacSha1(signKey, stringToSign);

            return result;
        }

        public string BuildAuthorizationString()
        {
            var requestTime = DateTimeOffset.UtcNow.AddMinutes(-1);

            var timestamp = new CosTimestamp(requestTime, TimeSpan.FromMinutes(3));//.FromSeconds(30));

            var authorizationKeys = new Dictionary<string, string>()
            {
                { "q-sign-algorithm",   SignAlgorithm},
                { "q-ak",               SecretId },
                { "q-sign-time",        timestamp.ToString() },
                { "q-key-time",         timestamp.ToString() },
                { "q-header-list",      HttpRequestMessage.Headers.ToHeaderListString() },
                { "q-url-param-list",   HttpRequestMessage.RequestUri.ToUrlParamListString() },
                { "q-signature",        CalculateSignature(timestamp) }
            };

            var result = string.Join("&", authorizationKeys.Select(k => $"{k.Key}={k.Value}"));
            return result;
        }
    }
}
