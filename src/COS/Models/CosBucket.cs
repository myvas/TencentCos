using System;
using System.Text.RegularExpressions;

namespace AspNetCore.TencentCos.Models
{
    /// <summary>
    /// COS存储桶。
    /// </summary>
    public class CosBucket
    {
        public BucketName BucketName { get; }

        public CosRegion CosRegion { get; }

        public CosBucket(BucketName bucketName, CosRegion cosRegion)
        {
            BucketName = bucketName;
            CosRegion = cosRegion;
        }

        public CosBucket(BucketName bucketName, string cosRegionCode)
            : this(bucketName, CosRegionBuilder.FindByCode(cosRegionCode))
        {
        }

        public CosBucket(string bucketName, string cosRegionCode)
            : this(new BucketName(bucketName), CosRegionBuilder.FindByCode(cosRegionCode))
        {
        }

        public CosBucket(string name, string appId, CosRegion cosRegion)
            : this(new BucketName(name, appId), cosRegion)
        {
        }

        public CosBucket(string name, string appId, string cosRegionCode)
            : this(new BucketName(name, appId), CosRegionBuilder.FindByCode(cosRegionCode))
        {
        }

        public CosBucket(BucketResult bucketResult)
            : this(new BucketName(bucketResult.Name), CosRegionBuilder.FindByCode(bucketResult.Location))
        {
        }

        /// <summary>
        /// 返回桶的访问URL地址，不包含/路径。
        /// </summary>
        public Uri ToUri(string uriScheme, string relativeUri)
        {
            if (BucketName == null)
            {
                throw new InvalidOperationException("BucketName is null.");
            }
            if (CosRegion == null)
            {
                throw new InvalidOperationException("CosRegion is null.");
            }
            var uri = CosRegion.ToUri(uriScheme, BucketName, relativeUri);
            return uri;
        }

        public Uri ToHttps(string relativeUri) => ToUri("https", relativeUri);
        public Uri ToHttp(string relativeUri) => ToUri("http", relativeUri);

        /// <summary>
        /// 解析存储桶的URL地址。
        /// </summary>
        /// <param name="url">存储桶的访问域名地址。</param>
        /// <returns><see cref="CosBucket"/>对象。</returns>
        public static CosBucket ParseUrl(string url)
        {
            var host = new Uri(url).Host;

            var regexPattern = @"(?<bucketName>[^-]+)-(?<appId>[^\.]+)\.cos\.(?<cosRegionCode>[^\.]+)\.myqcloud\.com";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
            var match = regex.Match(host);
            if (match.Success)
            {
                if (match.Groups["bucketName"].Success
                    && match.Groups["appId"].Success
                    && match.Groups["cosRegionCode"].Success)
                {
                    var bucketName = match.Groups["bucketName"].Value;
                    var appId = match.Groups["appId"].Value;
                    var cosRegionCode = match.Groups["cosRegionCode"].Value;
                    return new CosBucket(bucketName, appId, cosRegionCode);
                }
            }
            return null;
        }
    }
}
