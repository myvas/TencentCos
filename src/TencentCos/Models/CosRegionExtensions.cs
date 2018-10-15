using System;
using System.Collections.Generic;
using System.Text;

namespace Myvas.AspNetCore.TencentCos
{
    public static class CosRegionExtensions
    {
        /// <summary>
        /// 域名
        /// </summary>
        /// <param name="bucketName">带AppId</param>
        /// <returns></returns>
        public static string ToHost(this CosRegion cosRegion, BucketName bucketName)
        {
            return $"{bucketName.Name}-{bucketName.AppId}.cos.{cosRegion.Code}.myqcloud.com";
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="bucketName">带AppId</param>
        /// <param name="uriScheme">http, https, ...</param>
        /// <returns></returns>
        public static Uri ToUri(this CosRegion cosRegion, string uriScheme, BucketName bucketName, string relativeUri)
        {
            var baseUri = new Uri($"{uriScheme}://{cosRegion.ToHost(bucketName)}");
            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="bucketName">带AppId</param>
        /// <returns></returns>
        public static Uri ToHttps(this CosRegion cosRegion, BucketName bucketName, string relativeUri) => cosRegion.ToUri("https", bucketName, relativeUri);


        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="bucketName">带AppId</param>
        /// <returns></returns>
        public static Uri ToHttp(this CosRegion cosRegion, BucketName bucketName, string relativeUri) => cosRegion.ToUri("http", bucketName, relativeUri);
    }
}
