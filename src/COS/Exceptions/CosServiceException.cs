using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AspNetCore.TencentCos.Exceptions
{
    public enum CosErrorType
    {
        Client, Service, Unknown
    }

    public class CosServiceException : ApplicationException
    {
        private const long SerialVersionUID = 1L;

        public string RequestId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string RawResponseContent { get; set; }
        public string ErrorResponseXml { get; set; }

        public CosErrorType ErrorType { get; set; } = CosErrorType.Unknown;


    }
}
