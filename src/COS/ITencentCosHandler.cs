using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.TencentCos
{
    public interface ITencentCosHandler
    {
        /// <summary>
        /// 获取名下所有存储桶
        /// </summary>
        /// <returns></returns>
        Task<ListAllMyBucketsResult> AllBucketsAsync();

        /// <summary>
        /// 创建一个存储桶
        /// </summary>
        /// <param name="name"></param>
        /// <param name="region"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<CosBucket> PutBucketAsync(string name, string region, Dictionary<string, string> headers = null);

        /// <summary>
        /// 删除一个空的存储桶
        /// </summary>
        Task<bool> DeleteBucketAsync(CosBucket bucket);

        /// <summary>
        /// 删除一个空的存储桶
        /// </summary>
        /// <param name="name"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        Task<bool> DeleteBucketAsync(string name, string region);

        /// <summary>
        /// 上传文件到指定位置
        /// </summary>
        /// <param name="objectUri">存放位置，含文件名</param>
        /// <param name="content">文件内容</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<Uri> PutObjectAsync(string objectUri, Stream content, Dictionary<string, string> headers = null);

        /// <summary>
        /// 上传文件到指定位置
        /// </summary>
        /// <param name="baseUri">存储桶位置</param>
        /// <param name="relativeContainer">文件夹</param>
        /// <param name="objectName">文件名</param>
        /// <param name="content">文件内容</param>
        Task<Uri> PutObjectAsync(string baseUri, string relativeContainer, string objectName, Stream content, Dictionary<string, string> headers = null);

        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <remarks>
        /// 删除一个不存在的文件时，返回 204 No Content。
        /// </remarks>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<bool> DeleteObjectAsync(string uri);

        /// <summary>
        /// 读取文件内容
        /// </summary>
        Task<bool> GetObjectAsync(string requestUri, Action<Stream> actionSaveData, Dictionary<string, string> headers = null);

        /// <summary>
        /// 判断存储桶或文件是否存在
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string uri);
    }
}