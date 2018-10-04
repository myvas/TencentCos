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
        public void ShouldDeserializable_OneItem()
        {
            var path = "Data/ListAllMyBucketsResult.xml";
            using (TextReader reader = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(ListAllMyBucketsResult));
                var result = (ListAllMyBucketsResult)serializer.Deserialize(reader);

                Assert.NotNull(result);
                Assert.NotNull(result.Owner);
                Assert.NotNull(result.Buckets);
                Assert.True(result.Owner.DisplayName == "100000555120");
                Assert.True(result.Buckets.Count == 1);
                Assert.True(result.Buckets[0].Location == "ap-guangzhou");
                Assert.True(result.Buckets[0].Name == "dorr-1243608725");
            }

        }

        [Fact]
        public void ShouldDeserializable_TwoItems()
        {
            var path = "Data/ListAllMyBucketsResult2.xml";
            using (TextReader reader = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(ListAllMyBucketsResult));
                var result = (ListAllMyBucketsResult)serializer.Deserialize(reader);

                Assert.NotNull(result);
                Assert.NotNull(result.Owner);
                Assert.NotNull(result.Buckets);
                Assert.True(result.Owner.DisplayName == "100000555120");
                Assert.True(result.Buckets.Count == 2);
                Assert.True(result.Buckets[0].Location == "ap-guangzhou");
                Assert.True(result.Buckets[0].Name == "dorr-1243608725");
                Assert.True(result.Buckets[1].Location == "ap-shanghai");
                Assert.True(result.Buckets[1].Name == "d1rr-1243608725");
            }

        }
    }
}
