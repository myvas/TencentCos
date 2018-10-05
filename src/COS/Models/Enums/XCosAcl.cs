using System.ComponentModel.DataAnnotations;

namespace AspNetCore.TencentCos
{
    /// <summary>
    /// 访问权限
    /// </summary>
    public enum XCosAcl
    {
        /// <summary>
        /// private
        /// </summary>
        [Display(ShortName = "private", Name = "私有读写", Description = "需要进行身份验证后才能对object进行访问操作。", Order = 1)]
        PrivateReadAndWrite,

        /// <summary>
        /// public-read
        /// </summary>
        [Display(ShortName = "public-read", Name = "公有读私有写", Description = "可对object进行匿名读操作, 写操作需要进行身份验证。", Order = 2)]
        PublicReadPrivateWrite,

        /// <summary>
        /// public-read-write
        /// </summary>
        [Display(ShortName = "public-read-write", Name = "公有读写", Description = "可对object进行匿名读操作和写操作。", Order = 3)]
        PublicReadAndWrite,
    }
}