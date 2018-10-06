using AspNetCore.TencentCos;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace TencentCos.UnitTests
{
    public class ListBucketResultTests
    {
        [Fact]
        public void ShouldDeserializable_One()
        {
            var path = "Data/ListBucketResult.xml";
            using (TextReader reader = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(ListBucketResult));
                var result = (ListBucketResult)serializer.Deserialize(reader);

                Assert.NotNull(result);
                Assert.NotNull(result.Contents);
                Assert.NotNull(result.CommonPrefixes);
                Assert.Equal("zuhaotestnorth-1251668577", result.Name);
                Assert.Equal("1000", result.MaxKeys);
                Assert.Equal("/", result.Delimiter);
                Assert.False(result.IsTruncated);
                Assert.Equal(4,result.Contents.Count);
                Assert.Equal("testL", result.Contents[0].Key);
                Assert.Equal("1252375641", result.Contents[0].Owner.Id);
                Assert.Equal("STANDARD", result.Contents[0].XCosStorageClassShortName);
                Assert.Equal("testL1", result.Contents[1].Key);
            }
        }
    }
}
