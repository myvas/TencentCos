using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.TencentCos
{
    public class BucketName
    {
        public string Name { get; set; }
        public string AppId { get; set; }

        public BucketName(string bucketName)
        {
            (Name, AppId) = Parse(bucketName);
        }

        public BucketName(string name, string appId)
        {
            Name = name;
            AppId = appId;
        }

        public override string ToString()
        {
            return $"{Name}-{AppId}";
        }

        #region public methods
        /// <summary>
        /// Parse a string to <see cref="BucketName"/>
        /// </summary>
        /// <param name="bucketName">{Name}-{AppId}</param>
        /// <returns>(AppId, Name)</returns>
        public static (string Name, string AppId) Parse(string bucketName)
        {
            int pos = bucketName.LastIndexOf('-');
            if (pos > 0)
            {
                var name = bucketName.Substring(0, pos);
                var appId = bucketName.Substring(pos + 1);
                return (name, appId);
            }

            return (bucketName, "");
        }
        #endregion
    }
}
