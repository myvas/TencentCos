using System;
using System.Collections.Generic;
using System.Text;

namespace Myvas.AspNetCore.TencentCos
{
    public class CosTimestamp
    {
        public DateTimeOffset Time { get; set; }
        public TimeSpan Expires { get; set; }

        public CosTimestamp(DateTimeOffset time, TimeSpan expires)
        {
            Time = time;
            Expires = expires;
        }

        public override string ToString()
        {
            return $"{Time.ToUnixTimeSeconds()};{Time.Add(Expires).ToUnixTimeSeconds()}";
        }
    }
}
