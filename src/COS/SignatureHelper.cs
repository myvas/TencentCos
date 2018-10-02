using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace AspNetCore.TencentCos
{
    public static class SignatureHelper
    {
        #region private methods
        private static string SignAlgorithm = "sha1";

        private static string SignTimeStr(DateTimeOffset signatureTime, TimeSpan timeSpan)
        {
            return $"{signatureTime.ToUnixTimeSeconds()};{signatureTime.Add(timeSpan).ToUnixTimeSeconds()}";
        }
        #endregion

        public static string Authorization(HttpRequestMessage req, string secretId, string secretKey)
        {
            var queryStringCollection = HttpUtility.ParseQueryString(req.RequestUri.Query);

            var sortedQueryStrings = queryStringCollection.Cast<string>()
                .Select(k => new KeyValuePair<string, string>(k.ToLower(), queryStringCollection[k].ToLower()))
                .OrderBy(k => k.Key);
            var urlParamListStr = string.Join(";", sortedQueryStrings.Select(k => k.Key));

            var sortedHeaders = req.Headers.Select(k => new KeyValuePair<string, string>(
                k.Key.ToLower(), 
                Uri.EscapeDataString(k.Value.First())))
                .OrderBy(k => k.Key);
            var headerListStr = string.Join(";", sortedHeaders.Select(k => k.Key));

            var httpString = $"{req.Method.ToString().ToLower()}\n" +
                $"{req.RequestUri.LocalPath}\n" +
                $"{string.Join("&", sortedQueryStrings.Select(k => k.Key + "=" + k.Value))}\n" +
                $"{string.Join("&", sortedHeaders.Select(k => k.Key + "=" + k.Value))}\n";

            // Signature 计算
            var signTimeStr = SignTimeStr(DateTimeOffset.Now, TimeSpan.FromSeconds(30));

            var signKey = HashHelper.HmacSha1(secretKey, signTimeStr);
            var stringToSign = $"{SignAlgorithm}\n{signTimeStr}\n{HashHelper.Sha1(httpString)}\n";
            var signature = HashHelper.HmacSha1(signKey, stringToSign);

            var m = new Dictionary<string, string>()
            {
                { "q-sign-algorithm",   SignAlgorithm},
                { "q-ak",               secretId },
                { "q-sign-time",        signTimeStr },
                { "q-key-time",         signTimeStr },
                { "q-header-list",      headerListStr },
                { "q-url-param-list",   urlParamListStr },
                { "q-signature",     signature }
            };

            var authorizationStr = string.Join("&", m.Select(k => k.Key + "=" + k.Value));
            return authorizationStr;
        }
    }
}
