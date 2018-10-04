using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.TencentCos
{
    public class XCosGrantXParser
    {
        public List<string> Parse(string value)
        {
            return ExtractOwnerUins(value);
        }

        private List<string> ExtractOwnerUins(string s)
        {
            var result = new List<string>();

            var regexPattern = "id=\"(?<OwnerUin>[^;]+)\"";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
            var match = regex.Match(s);
            if (match.Success)
            {
                for (var i = 0; match.Success; i++, match = match.NextMatch())
                {
                    if (match.Groups["OwnerUin"].Success)
                    {
                        var item = match.Groups["OwnerUin"].Value;
                        result.Add(item);
                    }
                }
            }
            return result;
        }
    }
}
