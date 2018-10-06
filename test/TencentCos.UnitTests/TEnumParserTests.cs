using AspNetCore.TencentCos;
using Xunit;

namespace TencentCos.UnitTests
{
    public class TEnumParserTests
    {
        [Theory]
        [InlineData("private", XCosAcl.PrivateReadAndWrite)]
        [InlineData("public-read", XCosAcl.PublicReadPrivateWrite)]
        [InlineData("public-read-write", XCosAcl.PublicReadAndWrite)]
        public void Test_TEnumParser_ShortName(string shortName, XCosAcl expected)
        {
            var parser = new TEnumParser<XCosAcl>();
            Assert.Equal(parser.ParseFromShortName(shortName), expected);
        }

        [Theory]
        [InlineData("私有读写", XCosAcl.PrivateReadAndWrite)]
        [InlineData("公有读私有写", XCosAcl.PublicReadPrivateWrite)]
        [InlineData("公有读写", XCosAcl.PublicReadAndWrite)]
        public void Test_TEnumParser_DisplayName(string displayName, XCosAcl expected)
        {
            var parser = new TEnumParser<XCosAcl>();
            Assert.Equal(parser.ParseFromDisplayName(displayName), expected);
        }
    }
}
