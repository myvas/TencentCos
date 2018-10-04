using AspNetCore.TencentCos;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace TencentCos.UnitTests
{
    public class ListAllMyBucketsResultTests
    {
        [Fact]
        public void ShouldDeserializable_One()
        {
            var path = "Data/ListAllMyBucketsResult.xml";
            using (TextReader reader = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(ListAllMyBucketsResult));
                var result = (ListAllMyBucketsResult)serializer.Deserialize(reader);

                Assert.NotNull(result);
                Assert.NotNull(result.Owner);
                Assert.NotNull(result.Buckets);
                Assert.Equal("100000555120", result.Owner.DisplayName);
                Assert.Single(result.Buckets);
                Assert.Equal("ap-guangzhou", result.Buckets[0].Location);
                Assert.Equal("dorr-1243608725", result.Buckets[0].Name);
            }
        }

        [Fact]
        public void ShouldDeserializable_Two()
        {
            var path = "Data/ListAllMyBucketsResult2.xml";
            using (TextReader reader = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(ListAllMyBucketsResult));
                var result = (ListAllMyBucketsResult)serializer.Deserialize(reader);

                Assert.NotNull(result);
                Assert.NotNull(result.Owner);
                Assert.NotNull(result.Buckets);
                Assert.Equal("100000555120", result.Owner.DisplayName);
                Assert.Equal(2, result.Buckets.Count);
                Assert.Equal("ap-guangzhou", result.Buckets[0].Location);
                Assert.Equal("dorr-1243608725", result.Buckets[0].Name);
                Assert.Equal("ap-shanghai", result.Buckets[1].Location);
                Assert.Equal("d1rr-1243608725", result.Buckets[1].Name);
            }
        }
    }
}
