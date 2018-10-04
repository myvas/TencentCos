using AspNetCore.TencentCos.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace AspNetCore.TencentCos
{
    public class ListAllMyBucketsResult
    {
        public BucketOwnerResult Owner { get; set; }

        public List<BucketResult> Buckets { get; set; }
    }

    public class BucketOwnerResult
    {
        [XmlElement("ID")]
        public string Id { get; set; }

        public string DisplayName { get; set; }
    }

    [XmlType("Bucket")]
    public class BucketResult
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string CreateDate { get; set; }
    }
}
