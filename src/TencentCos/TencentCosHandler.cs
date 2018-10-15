using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Myvas.AspNetCore.TencentCos
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
        /// <remarks>See: https://cloud.tencent.com/document/product/436/8291 </remarks>
        /// <returns></returns>
        public async Task<ListAllMyBucketsResult> AllBucketsAsync()
        {
            var endpoint = TencentCosDefaults.ServiceEndpoint;
            var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
            using (var resp = await SendAsync(req))
            {
                if (!resp.IsSuccessStatusCode)
                {
                    RequestFailure(HttpMethod.Get, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                using (Stream sr = await resp.Content.ReadAsStreamAsync())
                {
                    var serializer = new XmlSerializer(typeof(ListAllMyBucketsResult));
                    var result = (ListAllMyBucketsResult)serializer.Deserialize(sr);
                    return result;
                }
            }
        }

        public async Task<ListBucketResult> AllObjectsAsync(string uri, string prefix, string maxKeys)
        {
            var query = new QueryBuilder()
            {
                { "prefix",         prefix},
                //{ "delimiter",      "" },
                //{ "encoding-type",  "" },
                //{ "marker",         "" },
                { "max-keys",       maxKeys },
            };
            uri = uri + query;

            var req = new HttpRequestMessage(HttpMethod.Get, uri);

            using (var resp = await SendAsync(req))
            {
                if (!resp.IsSuccessStatusCode)
                {
                    RequestFailure(HttpMethod.Get, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                using (Stream sr = await resp.Content.ReadAsStreamAsync())
                {
                    var serializer = new XmlSerializer(typeof(ListBucketResult));
                    var result = (ListBucketResult)serializer.Deserialize(sr);
                    return result;
                }
            }
        }

        /// <summary>
        /// 创建一个新的存储桶
        /// </summary>
        /// <remarks>See: https://cloud.tencent.com/document/product/436/7738 </remarks>
        /// <param name="header">自定义附加请求的标头</param>
        /// <returns>返回新创建的<see cref="CosBucket"/>，失败则抛出异常。</returns>
        public async Task<CosBucket> PutBucketAsync(CosBucket bucket, Dictionary<string, string> headers = null)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            var endpoint = bucket.ToHttps("/");
            var req = new HttpRequestMessage(HttpMethod.Put, endpoint);
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            using (var resp = await SendAsync(req))
            {
                if (!resp.IsSuccessStatusCode)
                {
                    RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                return bucket;
            }
        }

        /// <summary>
        /// 创建一个新的存储桶
        /// </summary>
        /// <remarks>See: https://cloud.tencent.com/document/product/436/7738 </remarks>
        /// <param name="name">存储桶名称，自动适应带或不带AppId，例如：dorr 或 dorr-1243608725 </param>
        /// <param name="cosRegionCode">存储桶所在区域（代码），例如：ap-guangzhou </param>
        /// <param name="header">自定义附加请求的标头</param>
        /// <returns>返回新创建的<see cref="CosBucket"/>，失败则抛出异常。</returns>
        public async Task<CosBucket> PutBucketAsync(string name, string cosRegionCode, Dictionary<string, string> headers = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(cosRegionCode))
            {
                throw new ArgumentNullException(nameof(cosRegionCode));
            }

            var bucketName = new BucketName(name);
            if (string.IsNullOrWhiteSpace(bucketName.AppId))
            {
                throw new ArgumentException($"Invalid argument[{nameof(name)}]", nameof(cosRegionCode));
            }

            var bucket = new CosBucket(bucketName, cosRegionCode);

            return await PutBucketAsync(bucket);
        }

        /// <summary>
        /// 删除一个空的存储桶
        /// </summary>
        /// <remarks>See: https://cloud.tencent.com/document/product/436/7732 </remarks>
        public async Task<bool> DeleteBucketAsync(CosBucket bucket)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            var endpoint = bucket.ToHttps("/");
            var req = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            using (var resp = await SendAsync(req))
            {
                if (!resp.IsSuccessStatusCode)
                {
                    RequestFailure(HttpMethod.Delete, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                return true;
            }
        }

        /// <summary>
        /// 删除一个空的存储桶
        /// </summary>
        /// <remarks>See: https://cloud.tencent.com/document/product/436/7732 </remarks>
        /// <param name="name">桶名称</param>
        /// <param name="region">桶在区域</param>
        /// <returns></returns>
        public async Task<bool> DeleteBucketAsync(string name, string cosRegionCode)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(cosRegionCode))
            {
                throw new ArgumentNullException(nameof(cosRegionCode));
            }

            var bucketName = new BucketName(name);
            if (string.IsNullOrWhiteSpace(bucketName.AppId))
            {
                throw new ArgumentException($"Invalid argument[{nameof(name)}]", nameof(cosRegionCode));
            }

            var bucket = new CosBucket(bucketName, cosRegionCode);

            return await DeleteBucketAsync(bucket);
        }

        /// <summary>
        /// 上传文件到指定位置
        /// https://cloud.tencent.com/document/product/436/7749
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public async Task<Uri> PutObjectAsync(string objectUri, Stream content, Dictionary<string, string> headers = null)
        {
            if (string.IsNullOrWhiteSpace(objectUri))
            {
                throw new ArgumentNullException(nameof(objectUri));
            }
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var req = new HttpRequestMessage(HttpMethod.Put, objectUri)
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

            return new Uri(objectUri);
        }

        /// <summary>
        /// 上传文件到指定位置
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="region"></param>
        /// <param name="objectName"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<Uri> PutObjectAsync(string baseUri, string relativeContainer, string objectName, Stream content, Dictionary<string, string> headers = null)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
            {
                throw new ArgumentNullException(nameof(baseUri));
            }
            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new ArgumentNullException(nameof(objectName));
            }

            var uri = new Uri(baseUri).Append(relativeContainer, objectName);
            return await PutObjectAsync(uri.ToString(), content, headers);
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <remarks>See: https://cloud.tencent.com/document/product/436/7753 </remarks>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Stream> GetObjectAsync(string requestUri, Action<Stream> actionHandleStream, Dictionary<string, string> headers = null)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, requestUri);

            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            var resp = await SendAsync(req);
            {

                if (!resp.IsSuccessStatusCode)
                {
                    RequestFailure(HttpMethod.Get, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                var stream = await resp.Content.ReadAsStreamAsync();
                actionHandleStream(stream);
                return stream;
            }
        }

        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> DeleteObjectAsync(string requestUri)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            using (var resp = await SendAsync(req))
            {
                if (!resp.IsSuccessStatusCode)
                {
                    RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                return true;
            }
        }

        /// <summary>
        /// 判断存储桶或文件是否存在
        /// </summary>
        public async Task<bool> ExistsAsync(string requestUri)
        {
            var req = new HttpRequestMessage(HttpMethod.Head, requestUri);

            using (var resp = await SendAsync(req))
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return false;
                    //RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                return true;
            }
        }

        private void RequestFailure(HttpMethod method, HttpStatusCode respStatusCode, string content)
        {
            using (var sr = new StringReader(content))
            {
                var serializer = new XmlSerializer(typeof(ErrorResult));
                var result = (ErrorResult)serializer.Deserialize(sr);

                var ex = new RequestFailureException(method, result)
                {
                    StatusCode = respStatusCode
                };
                throw ex;
            }
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req)
        {
            var secretId = _options.SecretId;
            var secretKey = _options.SecretKey;

            var signature = new CosSignature(req, secretId, secretKey);

            req.Headers.Host = req.RequestUri.Host;
            req.Headers.TryAddWithoutValidation("Authorization", signature.BuildAuthorizationString());

            return await _backchannel.SendAsync(req);
        }
    }
}
