using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.TencentCos
{
    public interface ITencentCosHandler
    {
        /// <summary>
        /// 获取请求者名下的所有存储空间列表（Bucket list）。
        /// </summary>
        /// <returns></returns>
        Task<Bucket[]> ListBucketsAsync();

        /// <summary>
        /// 在指定账号下创建一个 Bucket。
        /// 该 API 接口不支持匿名请求，您需要使用帯 Authorization 签名认证的请求才能创建新的 Bucket 。
        /// 创建 Bucket 的用户默认成为 Bucket 的持有者。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="region"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<Bucket> PutBucketAsync(string name, string region, Dictionary<string, string> headers = null);

        /// <summary>
        /// 在指定账号下删除 Bucket，删除之前要求 Bucket 内的内容为空，只有删除了 Bucket 内的信息，才能删除 Bucket 本身。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        Task DeleteBucketAsync(string name, string region);

        /// <summary>
        /// 将本地的对象（Object）上传至指定存储桶中。
        /// 该操作需要请求者对存储桶有写入权限。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task PutObjectAsync(string url, Stream content, Dictionary<string, string> headers = null);

        /// <summary>
        /// 在 COS 的 Bucket 中将一个文件（Object）删除。
        /// 该操作需要请求者对 Bucket 有 WRITE 权限。
        /// </summary>
        /// <remarks>
        /// 在 DELETE Object 请求中删除一个不存在的 Object，仍然认为是成功的，返回 204 No Content。
        /// </remarks>
        /// <param name="url"></param>
        /// <returns></returns>
        Task DeleteObjectAsync(string url);

        /// <summary>
        /// 在 COS 的存储桶中将一个文件（对象）下载至本地。
        /// 该操作需要请求者对目标对象具有读权限或目标对象对所有人都开放了读权限（公有读）。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<Stream> GetObjectAsync(string url);
    }
}