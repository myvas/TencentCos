using AspNetCore.TencentCos.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;

namespace AspNetCore.TencentCos
{
    /// <summary>
    /// COS请求失败错误描述。
    /// </summary>
    public class RequestFailureException : ApplicationException
    {
        public HttpMethod HttpMethod { get; set; }
        public ErrorResult ErrorResult { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public RequestFailureException(HttpMethod method, ErrorResult result) : base(result.Message)
        {
            HttpMethod = method;
            ErrorResult = result;
        }

        public RequestFailureException()
        {
        }

        protected RequestFailureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public RequestFailureException(string message)
            : base(message)
        {
        }

        public RequestFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override string Message => ErrorResult?.Message;

        public override string ToString() => $"{HttpMethod} {ErrorResult?.Resource} - {StatusCode}[{ErrorResult?.Code}]";
    }
}
