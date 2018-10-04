using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.TencentCos.Models
{
    public class CosRegionBuilder
    {
        public static List<CosRegion> CosRegions { get; private set; } = new List<CosRegion>()
        {
            new CosRegion("北京一区（华北）", "ap-beijing-1"),
            new CosRegion( "北京", "ap-beijing"),
            new CosRegion( "上海（华东）", "ap-shanghai"),
            new CosRegion( "广州（华南）", "ap-guangzhou"),
            new CosRegion( "成都（西南）", "ap-chengdu"),
            new CosRegion( "重庆", "ap-chongqing"),
            new CosRegion( "新加坡", "ap-singapore"),
            new CosRegion( "香港", "ap-hongkong"),
            new CosRegion( "多伦多", "na-toronto"),
            new CosRegion( "法兰克福", "eu-frankfurt"),
            new CosRegion( "孟买", "ap-mumbai"),
            new CosRegion( "首尔", "ap-seooul"),
            new CosRegion( "硅谷", "na-siliconvalley"),
            new CosRegion( "弗吉尼亚", "na-ashburn"),
            new CosRegion( "曼谷", "ap-bangkok"),
            new CosRegion( "莫斯科", "eu-moscow"),
        };

        public static CosRegion FindByCode(string code)
        {
            return CosRegions.Find(x => x.Code == code);
        }

        public static CosRegion FindByPartialName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            name = name.Trim().Replace("（", "").Replace("(", "").Replace("）", "").Replace(")", "");

            return CosRegions.Find(x => x.Name.Contains(name));
        }
    }
}
