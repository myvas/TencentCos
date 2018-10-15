using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Myvas.AspNetCore.TencentCos
{
    /// <summary>
    /// ContentMD5, ContentType, ContentLength
    /// </summary>
    public class CosContentHeaders //: HttpContentHeaders
    {
        public HttpContentHeaders HttpContentHeaders { get; private set; }

        public CosContentHeaders(HttpContentHeaders headers)
        {
            HttpContentHeaders = headers;
        }
    }
}
