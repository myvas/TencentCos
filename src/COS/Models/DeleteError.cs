using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.TencentCos.Models
{
    public class DeleteError : CosEntity
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
