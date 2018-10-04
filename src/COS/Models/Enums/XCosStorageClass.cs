using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AspNetCore.TencentCos.Models
{
    /// <summary>
    /// 访问
    /// </summary>
    public enum XCosStorageClass
    {
        [Display(ShortName = "STANDARD", Name = "标准存储", Order = 1)]
        Standard,

        [Display(ShortName = "STANDARD_IA", Name = "低频存储", Order = 2)]
        StandardIa,

        [Display(ShortName = "ARCHIVE", Name = "归档存储", Order = 3)]
        Archive
    }
}
