using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace AspNetCore.TencentCos
{
    internal static class SignatureExtensions
    {
        public static string ToHttpMethodString(this HttpMethod httpMethod)
        {
            return httpMethod.ToString().ToLowerInvariant();
        }

        public static string ToHttpUriString(this Uri requestUri)
        {
            return requestUri.LocalPath;
        }

        private static IOrderedEnumerable<KeyValuePair<string, string>> GetQueryCollection(Uri requestUri)
        {
            var queryStringCollection = HttpUtility.ParseQueryString(requestUri.Query);

            var sortedQueryCollection = queryStringCollection.Cast<string>()
                .Select(x => new KeyValuePair<string, string>(x.ToLower(), queryStringCollection[x].ToLower()))
                .OrderBy(x => x.Key);

            return sortedQueryCollection;
        }

        public static string ToUrlParamListString(this Uri requestUri)
        {
            var sortedQueryStrings = GetQueryCollection(requestUri);
            var result = string.Join(";", sortedQueryStrings.Select(x => x.Key));
            return result;
        }

        public static string ToHttpParameterString(this Uri requestUri)
        {
            var sortedQueryStrings = GetQueryCollection(requestUri);
            var result = string.Join("&", sortedQueryStrings.Select(x => $"{x.Key}={x.Value}"));
            return result;
        }

        private static IOrderedEnumerable<KeyValuePair<string, string>> GetHeaders(HttpRequestHeaders headers)
        {
            var sortedHeaders = headers.Select(x => new KeyValuePair<string, string>(
                x.Key.ToLower(),
                Uri.EscapeDataString(string.Join(";", x.Value))))
                .OrderBy(x => x.Key);
            return sortedHeaders;
        }


        public static string ToHttpHeaderString(this HttpRequestHeaders headers)
        {
            var sortedHeaders = GetHeaders(headers);
            var result = string.Join("&", sortedHeaders.Select(x => $"{x.Key}={x.Value}"));
            return result;
        }

        public static string ToHeaderListString(this HttpRequestHeaders headers)
        {
            var sortedHeaders = GetHeaders(headers);
            var result = string.Join(";", sortedHeaders.Select(x => x.Key));
            return result;
        }

        public static string ToHttpString(this HttpRequestMessage req)
        {
            var httpMethod = req.Method;
            var requestUri = req.RequestUri;
            var headers = req.Headers;

            return $"{httpMethod.ToHttpMethodString()}\n{requestUri.ToHttpUriString()}\n{requestUri.ToHttpParameterString()}\n{headers.ToHttpHeaderString()}\n";
        }
    }
}
