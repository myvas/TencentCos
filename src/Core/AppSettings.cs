using System;

namespace QCloud
{
    /// <summary>
    /// 腾讯云应用密钥配置。
    /// </summary>
    public sealed class AppSettings
    {
        /// <summary>
        /// 应用ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 密钥ID。
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// 密钥KEY。
        /// </summary>
        public string SecretKey { get; set; }
    }
}
