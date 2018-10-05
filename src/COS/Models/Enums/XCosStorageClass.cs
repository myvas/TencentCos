using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AspNetCore.TencentCos
{
    /// <summary>
    /// 访问
    /// </summary>
    public enum XCosStorageClass
    {
        [Display(ShortName = "STANDARD", Name = "标准存储", Description = "适用场景：热点视频、社交图片、移动应用、游戏程序、动态网站。", Order = 1)]
        Standard,

        [Display(ShortName = "STANDARD_IA", Name = "低频存储", Description = "适用场景：网盘数据、大数据分析、政企业务数据、低频档案、监控数据。", Order = 2)]
        StandardIa,

        [Display(ShortName = "ARCHIVE", Name = "归档存储", Description = "适用场景：档案数据、医疗影像、科学资料、影视素材。", Order = 3)]
        Archive
    }
}
