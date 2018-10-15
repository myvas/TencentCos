using System;
using System.Collections.Generic;
using System.Text;

namespace Myvas.AspNetCore.TencentCos
{
    public class CosRegion
    {
        /// <summary>
        /// 地域/显示名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地域简称/代码
        /// </summary>
        public string Code { get; set; }

        public CosRegion(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
