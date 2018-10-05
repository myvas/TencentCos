using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace AspNetCore.TencentCos.Headers
{
    /// <summary>
    /// Connection, Date, Server, ETag
    /// </summary>
    public class CosResponseHeaders //: HttpResponseHeaders
    {
        public const string XCosRequestIdDescriptor = "x-cos-request-id";
        public const string XCosTraceIdDescriptor = "x-cos-trace-id";

        public HttpResponseHeaders HttpResponseHeaders { get; private set; }

        public CosResponseHeaders(HttpResponseHeaders headers)
        {
            HttpResponseHeaders = headers;
        }

        public string XCosRequestId
        {
            get
            {
                return HttpResponseHeaders.GetValues(XCosRequestIdDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            }
            set
            {
                HttpResponseHeaders.TryAddWithoutValidation(XCosRequestIdDescriptor, value);
            }
        }

        public string XCosTraceId
        {
            get
            {
                return HttpResponseHeaders.GetValues(XCosTraceIdDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            }
            set
            {
                HttpResponseHeaders.TryAddWithoutValidation(XCosTraceIdDescriptor, value);
            }
        }
    }
}
