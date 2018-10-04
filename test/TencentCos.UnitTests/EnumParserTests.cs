using AspNetCore.TencentCos;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;
using AspNetCore.TencentCos.Models;

namespace TencentCos.UnitTests
{
    public class EnumParserTests
    {
        [Fact]
        public void ShouldPass_XCosAcl()
        {
            var parser = new TEnumParser<XCosAcl>();

            Assert.True(parser.ParseFromShortName("private") == XCosAcl.PrivateReadAndWrite);
            Assert.True(parser.ParseFromShortName("public-read") == XCosAcl.PublicReadPrivateWrite);
            Assert.True(parser.ParseFromShortName("public-read-write") == XCosAcl.PublicReadAndWrite);

            Assert.True(parser.ParseFromDisplayName("私有读写") == XCosAcl.PrivateReadAndWrite);
            Assert.True(parser.ParseFromDisplayName("公有读私有写") == XCosAcl.PublicReadPrivateWrite);
            Assert.True(parser.ParseFromDisplayName("公有读写") == XCosAcl.PublicReadAndWrite);
        }
    }
}
