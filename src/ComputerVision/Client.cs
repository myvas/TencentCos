using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QCloud.ComputerVision
{
    /// <summary>
    /// 图像处理客户端。
    /// </summary>
    public class Client
    {
        private const string TaggingApiAddress = "https://recognition.image.myqcloud.com/v1/detection/imagetag_detect";
        private static Random _random = new Random();

        private readonly AppSettings _config;
        private readonly HttpClient _backChannel;

        public Client(AppSettings conf, HttpClient backChannel)
        {
            _config = conf;
            _backChannel = backChannel ?? throw new ArgumentNullException("backChannel");
        }

        /// <summary>
        /// 图像标签操作。
        /// </summary>
        /// <param name="url">图像地址</param>
        /// <returns></returns>
        public async Task<List<ImageTag>> AnalyzeImageTagsAsync(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, TaggingApiAddress);
            req.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                appid =_config.AppId,
                url = url,
            }), Encoding.UTF8, "application/json");
            req.Headers.TryAddWithoutValidation("Authorization", GenerateAuth());
            var resp = await _backChannel.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Operation returned an invalid status code {resp.StatusCode}\n{await resp.Content.ReadAsStringAsync()}");
            }
            var payload = JObject.Parse(await resp.Content.ReadAsStringAsync());
            var data = from c in payload["tags"]
                       select new ImageTag()
                       {
                           Name = (string)c["tag_name"],
                           Score = (int)c["tag_confidence"]
                       };
            return data.ToList();
        }

        // https://cloud.tencent.com/document/product/865/17723
        private string GenerateAuth()
        {
            var parameters = new Dictionary<string, object>()
            {
                ["a"] = _config.AppId,
                ["k"] = _config.SecretId,
                ["e"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 10,// 默认为0，有可能造成请求的签名直接失败。
                ["t"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ["r"] = _random.Next(),
                ["f"] = "",
            };
            var rawText = string.Join("&", parameters.Select(k => $"{k.Key}={k.Value.ToString()}"));
            var encrypted = HashHMACSHA1(_config.SecretKey, rawText);
            var rawTextBytes = Encoding.UTF8.GetBytes(rawText);

            var bytes = new byte[encrypted.Length + rawTextBytes.Length];
            Array.Copy(encrypted, bytes, encrypted.Length);
            Array.Copy(rawTextBytes, 0, bytes, encrypted.Length, rawTextBytes.Length);
            return Convert.ToBase64String(bytes);
        }

        private static byte[] HashHMACSHA1(string key, string content)
        {
            var hash = new HMACSHA1(Encoding.UTF8.GetBytes(key));
            return hash.ComputeHash(Encoding.UTF8.GetBytes(content));
        }
    }
}
