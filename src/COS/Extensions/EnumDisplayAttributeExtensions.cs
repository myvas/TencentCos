using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.TencentCos
{

    internal static class EnumDisplayAttributeExtensions
    {
        /// <summary>
        /// Flags supported!
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetShortName(this Enum enumValue)
        {
            List<string> results = new List<string>();

            var fieldNames = enumValue.ToString();
            foreach (var fieldName in fieldNames.Split(new[] { ',', ' ' }))
            {
                var fi = enumValue.GetType().GetField(fieldName);

                var attributes = (DisplayAttribute[])fi?.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attributes != null && attributes.Length > 0)
                    results.Add(attributes[0].ShortName);
                else
                    results.Add(enumValue.ToString());
            }
            return string.Join(", ", results);
        }


        /// <summary>
        /// Flags supported!
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            List<string> results = new List<string>();

            var fieldNames = enumValue.ToString();
            foreach (var fieldName in fieldNames.Split(new[] { ',', ' ' }))
            {
                var fi = enumValue.GetType().GetField(fieldName);

                var attributes = (DisplayAttribute[])fi?.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attributes != null && attributes.Length > 0)
                    results.Add(attributes[0].GetName());
                else
                    results.Add(enumValue.ToString());
            }
            return string.Join(", ", results);
        }
    }
}
