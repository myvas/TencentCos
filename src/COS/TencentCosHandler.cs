using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AspNetCore.TencentCos
{
    /// <summary>
    /// COS客户端，执行COS的请求。
    /// </summary>
    public class TencentCosHandler : ITencentCosHandler
    {
        private readonly HttpClient _backchannel;
        private readonly TencentCosOptions _options;
        private readonly ILogger _logger;

        /// <summary>
        /// 初始化新的<see cref="TencentCosHandler"/>实例。
        /// </summary>
        /// <param name="optionsAccessor">密钥配置</param>
        /// <param name="backchannel">自定义<see cref="HttpClient"/>实例。</param>
        public TencentCosHandler(IOptions<TencentCosOptions> optionsAccessor,
            ILogger<TencentCosHandler> logger)
        {
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _backchannel = _options.Backchannel ?? new HttpClient();
        }

        /// <summary>
        /// ListBucketsAsync返回所有存储空间列表。
        /// </summary>
        /// <returns></returns>
        public async Task<Bucket[]> ListBucketsAsync()
        {
            const string serviceEndpoint = "https://service.cos.myqcloud.com/";

            var req = new HttpRequestMessage(HttpMethod.Get, serviceEndpoint);
            using (var resp = await SendAsync(req))
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    RequestFailure(HttpMethod.Get, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                var resultXml = await resp.Content.ReadAsStringAsync();
                var doc = new XmlDocument();
                doc.LoadXml(resultXml);
                var buckets = new List<Bucket>();
                foreach (XPathNavigator elem in doc.DocumentElement.CreateNavigator().Select("//Buckets/Bucket"))
                {
                    var fullname = elem.SelectSingleNode("Name").InnerXml;
                    var pos = fullname.LastIndexOf("-");
                    var bucket = new Bucket(
                        appId: fullname.Substring(pos + 1),
                        name: fullname.Substring(0, pos),
                        region: elem.SelectSingleNode("Location").InnerXml);
                    buckets.Add(bucket);
                }
                return buckets.ToArray();
            }
        }

        /// <summary>
        /// PutBucketAsync创建一个新的存储桶(Bucket)。
        /// https://cloud.tencent.com/document/product/436/7738
        /// </summary>
        /// <param name="name">桶名称</param>
        /// <param name="region">桶在区域</param>
        /// <param name="header">自定义附加请求的标头</param>
        /// <returns></returns>
        public async Task<Bucket> PutBucketAsync(string name, string region, Dictionary<string, string> headers = null)
        {
            var bucket = new Bucket(_options.AppId, name, region);
            var endpoint = bucket.Url + "/";
            var req = new HttpRequestMessage(HttpMethod.Put, endpoint);
            if (headers?.Count > 0)
            {
                foreach (var header in headers)
                {
                    req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            using (var resp = await SendAsync(req))
            {
                var payload = await resp.Content.ReadAsStringAsync();
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }
                return bucket;
            }
        }

        /// <summary>
        /// DeleteBucketAsync删除一个指定的存储桶(Bucket)。
        /// </summary>
        /// <param name="name">桶名称</param>
        /// <param name="region">桶在区域</param>
        /// <returns></returns>
        public async Task DeleteBucketAsync(string name, string region)
        {
            var bucket = new Bucket(_options.AppId, name, region);
            var endpoint = bucket.Url + "/";
            var req = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            using (var resp = await SendAsync(req))
            {
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        break;
                    default:
                        RequestFailure(HttpMethod.Delete, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                        break;
                }
            }
        }

        /// <summary>
        /// 上传到指定存储桶。
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="region"></param>
        /// <param name="objectName"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task PutObjectAsync(string bucketName, string region, string objectName, Stream content, Dictionary<string, string> headers = null)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new ArgumentNullException(nameof(objectName));
            }

            var host = BuildHostName(bucketName, region);
            var url = $"{host}/{objectName}";
            await PutObjectAsync(url, content, headers);
        }

        private string BuildHostName(string bucketName, string region)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            if (!bucketName.EndsWith($"-{_options.AppId}"))
            {
                bucketName = $"{bucketName}-{_options.AppId}";
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentNullException(nameof(region));
            }

            var host = $"{bucketName}.cos.{region}.myqcloud.com";
            return host;
        }

        /// <summary>
        /// 上传文件到指定的URL。
        /// https://cloud.tencent.com/document/product/436/7749
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public async Task PutObjectAsync(string url, Stream content, Dictionary<string, string> headers = null)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var req = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StreamContent(content)
            };

            if (headers?.Count > 0)
            {
                foreach (var header in headers)
                {
                    req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            using (var resp = await SendAsync(req))
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// GetObjectAsync读取上传的文件内容。
        /// https://cloud.tencent.com/document/product/436/7753
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Stream> GetObjectAsync(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            using (var resp = await SendAsync(req))
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    RequestFailure(HttpMethod.Get, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }
                return await resp.Content.ReadAsStreamAsync();
            }
        }

        /// <summary>
        /// DeleteObjectAsync删除指定的文件。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task DeleteObjectAsync(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, url);

            using (var resp = await SendAsync(req))
            {
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        break;
                    default:
                        RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                        break;
                }
            }
        }

        private void RequestFailure(HttpMethod method, HttpStatusCode respStatusCode, string respContent)
        {
            var doc = new XmlDocument();
            doc.LoadXml(respContent);
            var root = doc.DocumentElement.CreateNavigator().SelectSingleNode("//Error");
            var ex = new RequestFailureException(method.ToString(), root.SelectSingleNode("Message").InnerXml)
            {
                HttpStatusCode = (int)respStatusCode,
                ErrorCode = root.SelectSingleNode("Code").InnerXml,
                ResourceURL = root.SelectSingleNode("Resource").InnerXml,
                RequestId = root.SelectSingleNode("RequestId").InnerXml,
                TraceId = root.SelectSingleNode("TraceId").InnerXml,
            };
            throw ex;
        }



        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req)
        {
            var secretId = _options.SecretId;
            var secretKey = _options.SecretKey;

            var authorization = SignatureHelper.Authorization(req, secretId, secretKey);

            req.Headers.Host = req.RequestUri.Host;
            req.Headers.TryAddWithoutValidation("Authorization", authorization);

            return await _backchannel.SendAsync(req);
        }
    }
}
