using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace AspNetCore.TencentCos.Headers
{
    public class PutObjectRequestHeaders : CosRequestHeaders
    {
        public const string XCosAclDescriptor = "x-cos-acl";
        public const string XCosStorageClassDescriptor = "x-cos-storage-class";
        public const string XCosGrantReadDescriptor = "x-cos-grant-read";
        public const string XCosGrantWriteDescriptor = "x-cos-grant-write";
        public const string XCosGrantFullControlDescriptor = "x-cos-grant-full-control";
        public const string XCosServerSideEncryptionDescriptor = "x-cos-server-side-encryption";
        public const string XCosServerSideEncryption_Aes256 = "AES256";

        public const string XCosMetaPrefix = "x-cos-meta-";

        public PutObjectRequestHeaders(HttpRequestHeaders headers)
            : base(headers)
        {
        }

        public bool XCosServerSideEncryption
        {
            get
            {
                var s = HttpRequestHeaders.GetValues(XCosServerSideEncryptionDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                return string.Compare(XCosServerSideEncryption_Aes256, s, true) == 0;
            }
            set
            {
                if (value)
                    HttpRequestHeaders.TryAddWithoutValidation(XCosServerSideEncryptionDescriptor, XCosServerSideEncryption_Aes256);
            }
        }

        public XCosStorageClass XCosStorageClass
        {
            get
            {
                var s = HttpRequestHeaders.GetValues(XCosStorageClassDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                try
                {
                    return new TEnumParser<XCosStorageClass>().ParseFromShortName(s);
                }
                catch
                {
                    return XCosStorageClass.Standard;
                }
            }
            set
            {
                HttpRequestHeaders.TryAddWithoutValidation(XCosStorageClassDescriptor, value.GetShortName());
            }
        }

        public XCosAcl XCosAcl
        {
            get
            {
                var s = HttpRequestHeaders.GetValues(XCosAclDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                try
                {
                    return new TEnumParser<XCosAcl>().ParseFromShortName(s);
                }
                catch
                {
                    return XCosAcl.PrivateReadAndWrite;
                }
            }
            set
            {
                HttpRequestHeaders.TryAddWithoutValidation(XCosAclDescriptor, value.GetShortName());
            }
        }

        public List<string> XCosGrantRead
        {
            get
            {
                var s = HttpRequestHeaders.GetValues(XCosGrantReadDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                return new XCosGrantXParser().Parse(s);
            }
            set
            {
                var s = string.Join(";", value.Select(x => $"id={x}"));
                HttpRequestHeaders.TryAddWithoutValidation(XCosGrantReadDescriptor, s);
            }
        }

        public List<string> XCosGrantWrite
        {
            get
            {
                var s = HttpRequestHeaders.GetValues(XCosGrantWriteDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                return new XCosGrantXParser().Parse(s);
            }
            set
            {
                var s = string.Join(";", value.Select(x => $"id={x}"));
                HttpRequestHeaders.TryAddWithoutValidation(XCosGrantWriteDescriptor, s);
            }
        }

        public List<string> XCosGrantFullControl
        {
            get
            {
                var s = HttpRequestHeaders.GetValues(XCosGrantFullControlDescriptor).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                return new XCosGrantXParser().Parse(s);
            }
            set
            {
                var s = string.Join(";", value.Select(x => $"id={x}"));
                HttpRequestHeaders.TryAddWithoutValidation(XCosGrantFullControlDescriptor, s);
            }
        }

        //public string XCosMetaX
        public List<string> GetMeta(string name)
        {
            var descriptor = $"{XCosMetaPrefix}{name.ToLowerInvariant()}";
            return HttpRequestHeaders.GetValues(descriptor).ToList();
        }

        public void SetMeta(string name, params string[] values)
        {
            var descriptor = $"{XCosMetaPrefix}{name.ToLowerInvariant()}";
            if (values.Length > 0)
            {
                HttpRequestHeaders.TryAddWithoutValidation(descriptor, values.ToArray());
            }
        }
    }
}
