using System;
using System.Net.Http;

namespace AspNetCore.TencentCos
{
    /// <summary>
    /// 腾讯云应用密钥配置。
    /// </summary>
    public class TencentCosOptions
    {
        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        /// <summary>
        /// Used to communicate with the remote cos server.
        /// </summary>
        public HttpClient Backchannel { get; set; }

        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with the remote cos server.
        /// </summary>
        public TimeSpan BackchannelTimeout { get; set; }

        /// <summary>
        /// Gets or sets the time limit for completing the cos operation/flow (15 minutes by default).
        /// </summary>
        public TimeSpan RemoteCosTimeout { get; set; } = TimeSpan.FromMinutes(15);

        public TencentCosOptions()
        {

        }

        public virtual void Validate()
        {
            if (string.IsNullOrEmpty(SecretId))
                throw new ArgumentNullException(nameof(SecretId));
            if (string.IsNullOrEmpty(SecretId))
                throw new ArgumentNullException(nameof(SecretKey));
        }
    }
}
