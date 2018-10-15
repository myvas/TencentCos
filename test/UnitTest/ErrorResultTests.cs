using Myvas.AspNetCore.TencentCos;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace UnitTest
{
    public class ErrorResultTests
    {
        [Fact]
        public void ShouldDeserializable()
        {
            var path = "Data/Error.xml";
            using (TextReader reader = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(ErrorResult));
                var result = (ErrorResult)serializer.Deserialize(reader);

                Assert.NotNull(result);
                Assert.Equal("AccessDenied", result.Code);
                Assert.Equal("dorr-1243608725.cos.ap-guangzhou.myqcloud.com/test/≤‚ ‘.jpg", result.Resource);
                Assert.Equal("NWJiNWUwOGZfY2JhMzNiMGFfYTdkZF80NTI1MWE=", result.RequestId);
            }
        }
    }
}
