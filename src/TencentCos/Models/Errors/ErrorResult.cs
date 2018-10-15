using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Myvas.AspNetCore.TencentCos
{
    /// <summary>
    /// 错误信息返回内容
    /// </summary>
    [XmlRoot("Error")]    
    public class ErrorResult
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 错误信息：包含具体的错误信息。
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 资源地址：Bucket地址或者Object地址。
        /// </summary>
        public string Resource { get; set; }
        /// <summary>
        /// 请求ID：当请求发送时，服务端将会自动为请求生成一个唯一的 ID。使用遇到问题时，request-id能更快地协助 COS 定位问题。
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// 错误ID：当请求出错时，服务端将会自动为这个错误生成一个唯一的 ID。使用遇到问题时，trace-id能更快地协助 COS 定位问题。当请求出错时，trace-id与request-id一一对应。
        /// </summary>
        public string TraceId { get; set; }
    }
}
