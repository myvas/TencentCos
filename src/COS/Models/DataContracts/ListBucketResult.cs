using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.TencentCos
{
    /// <summary>
    /// 保存 Get Bucket/List Objects 请求结果的所有信息
    /// </summary>
    public class ListBucketResult
    {
        /// <summary>
        /// 存储桶名称，例如：zuhaotestnorth-1251668577
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编码格式，例如：url
        /// </summary>
        [XmlElement("Encoding-Type")]
        public string EncodingType { get; set; }

        /// <summary>
        ///  前缀匹配，用于筛选对象
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 默认以 UTF-8 二进制顺序列出条目，所有列出条目从 marker 开始
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// 单次响应请求内返回结果的最大的条目数量，例如：1000
        /// </summary>
        public string MaxKeys { get; set; }

        /// <summary>
        /// 定界符，为一个符号。
        /// 如果有 Prefix，则从 Prefix 到 delimiter 之间的相同路径将归为一类，定义为 Common Prefix。
        /// 如果没有 Prefix，则从路径起点开始。
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// 响应请求条目是否被截断
        /// </summary>
        public bool IsTruncated { get; set; }

        /// <summary>
        /// 假如返回条目被截断，则该 NextMarker 就是下一个条目的起点，例如：yyyyyy.png
        /// </summary>
        public string NextMarker { get; set; }

        /// <summary>
        /// 元数据信息
        /// </summary>
        [XmlElement("Contents")]
        public List<CloudObjectMetadata> Contents { get; set; }

        /// <summary>
        /// 将 Prefix 到 delimiter 之间的相同路径归为一类，定义为 Common Prefix
        /// </summary>
        [XmlArrayItem("Prefix")]
        public List<string> CommonPrefixes { get; set; }
    }

    /// <summary>
    /// 存储对象元数据
    /// </summary>
    public class CloudObjectMetadata
    {
        /// <summary>
        /// 存储对象的Key
        /// （1）文件夹，例如：design/logo/
        /// （2）文件，例如：design/logo/logo.png
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 最后修改时间，例如：2017-06-23T12:33:26.000Z
        /// </summary>
        public string LastModified { get; set; }
        public DateTimeOffset GetLastModified() { return DateTimeOffset.Parse(LastModified); }

        /// <summary>
        /// 存储对象变更校验值(MD5 算法)，例如：\"79f2a852fac7e826c9f4dbe037f8a63b\"
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// 存储对象大小， 单位为Byte，例如：1048576
        /// </summary>
        public string Size { get; set; }
        public int GetSize() { return int.Parse(Size); }

        /// <summary>
        /// 容器所有者。
        /// </summary>
        /// <remarks>对于存储对象来说，其容器就是存储桶，存储桶的所有者就是应用，应用的唯一标识是AppId</remarks>
        public ContainerOwner Owner { get; set; }

        /// <summary>
        /// 存储桶类型, 枚举类型的DisplayAttribute.ShortName的值
        /// </summary>
        [XmlElement("StorageClass")]
        public string XCosStorageClassShortName { get; set; }
        public XCosStorageClass GetStorageClass() { return new TEnumParser<XCosStorageClass>().ParseFromShortName(XCosStorageClassShortName); }
    }

    public class ContainerOwner
    {
        [XmlElement("ID")]
        public string Id { get; set; }
    }
}