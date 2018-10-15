using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Myvas.AspNetCore.TencentCos
{

    /// <summary>
    /// Host, Expect, Date, Authorization
    /// </summary>
    public class CosRequestHeaders //: HttpRequestHeaders
    {
        public HttpRequestHeaders HttpRequestHeaders { get; private set; }

        public CosRequestHeaders(HttpRequestHeaders headers)
        {
            HttpRequestHeaders = headers;
        }
    }
}
